using AdvertApi.Models;
using System;
using Amazon.DynamoDBv2.DataModel;

namespace AdvertApi.Services
{
    [DynamoDBTable("Adverts")]
    public class AdvertDBModel
    {
        [DynamoDBHashKey]
        public string Id { get; set; }

        [DynamoDBProperty]
        public string Title { get; set; }

        [DynamoDBProperty]
        public string Description { get; set; }

        [DynamoDBProperty]
        public double Price { get; set; }

        [DynamoDBProperty]
        public DateTime CreationDateTime { get; set; }

        [DynamoDBProperty]
        public AdvertStatus Status { get; set; }

        [DynamoDBProperty] 
        public string FilePath { get; set; }
    }
}
