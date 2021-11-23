using AdvertApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace AdvertApi.Services
{
    public class DynamoDBAdvertStorage : IAdvertStorageService
    {
        private readonly IMapper _mapper;

        public DynamoDBAdvertStorage(IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<string> Add(AdvertModel model)
        {
            var dbModel = _mapper.Map<AdvertDBModel>(model);

            dbModel.Id = new Guid().ToString();
            dbModel.CreationDateTime = DateTime.UtcNow;
            dbModel.Status = AdvertStatus.Pending;


            using (var client = new AmazonDynamoDBClient())
            {
                using (var context = new DynamoDBContext(client))
                {
                    await context.SaveAsync(dbModel);

                }
            }

            return dbModel.Id;
        }

        public async Task<bool> CheckHealthAsync()
        {
            Console.WriteLine("Health checking...");
            using (var client = new AmazonDynamoDBClient())
            {
                var tableData = await client.DescribeTableAsync("Adverts");
                return string.Compare(tableData.Table.TableStatus, "active", true) == 0;
            }
        }

        public async Task<bool> Confirm(ConfirmAdvertModel model)
        {
            using (var client = new AmazonDynamoDBClient())
            {
                using (var context = new DynamoDBContext(client))
                {
                    var record = await context.LoadAsync<AdvertDBModel>(model.Id);
                    if (record == null) throw new KeyNotFoundException($"A record with ID={model.Id} was not found.");
                    if (model.Status == AdvertStatus.Active)
                    {
                        record.FilePath = model.FilePath;
                        record.Status = AdvertStatus.Active;
                        await context.SaveAsync(record);
                        return true;
                    }
                    else
                    {
                        await context.DeleteAsync(record);
                        return false;
                    }
                }
            }
        }
    }
}
