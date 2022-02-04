using MongoDB.Bson.Serialization.Attributes;

namespace ExchangeModels.Projections
{
    public class CurrencyMaxMin
    {
        [BsonElement("_id")]
        public string Symbol { get; set; }

        [BsonElement("highest")]
        public double Highest { get; set; }

        [BsonElement("lowest")]
        public double Lowest { get; set; }
    }
}