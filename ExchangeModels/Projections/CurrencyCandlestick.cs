using MongoDB.Bson.Serialization.Attributes;

namespace ExchangeModels.Projections
{
    public class CurrencyCandlestick
    {
        [BsonElement("_id")]
        public CandlestickId CandlestickId { get; set; }

        [BsonElement("close")]
        public double Close { get; set; }

        [BsonElement("high")]
        public double High { get; set; }

        [BsonElement("low")]
        public double Low { get; set; }

        [BsonElement("open")]
        public double Open { get; set; }
    }
}