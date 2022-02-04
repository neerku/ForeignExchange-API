using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ExchangeModels.Projections
{
    public class CurrencyEMA
    {
        [BsonElement("_id")]
        public CandlestickId CandlestickId { get; set; }

        [BsonElement("ema_10")]
        public double EMA_10 { get; set; }

        [BsonElement("ema_5")]
        public double EMA_5 { get; set; }

        [BsonElement("price")]
        public double Price { get; set; }

        [BsonElement("sma_10")]
        public double SMA_10 { get; set; }

        [BsonElement("sma_3")]
        public double SMA_3 { get; set; }

        [BsonElement("time")]
        public DateTime Time { get; set; } = DateTime.UtcNow;
    }
}