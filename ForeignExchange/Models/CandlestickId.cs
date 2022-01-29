using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ForeignExchange.Models
{
    public class CandlestickId
    {
        [BsonElement("symbol")]
        public string Symbol { get; set; }

        [BsonElement("time")]
        public DateTime Time { get; set; }
    }
}
