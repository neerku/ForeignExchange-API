using System;
using MongoDB.Bson.Serialization.Attributes;

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
