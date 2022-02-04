using MongoDB.Bson.Serialization.Attributes;

namespace ExchangeModels.Projections
{
    public class CurrencyMaxMin
    {
        [BsonElement("highest")]
        public double Highest { get; set; }

        [BsonElement("lowest")]
        public double Lowest { get; set; }
    }
}