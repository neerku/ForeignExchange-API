using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ExchangeModels.Projections
{
    public class Currency
    {
        [BsonElement("price")]
        public double Price { get; set; }

        [BsonElement("symbol")]
        public string Symbol { get; set; }

        [BsonElement("time")]
        public DateTime Time { get; set; }
    }
}