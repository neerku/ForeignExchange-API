using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ExchangeModels
{
    public class CandlestickId
    {
        [BsonElement("symbol")]
        public string Symbol { get; set; }

        [BsonElement("time")]
        public DateTime Time { get; set; }
    }
}