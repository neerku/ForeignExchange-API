using MongoDB.Bson.Serialization.Attributes;

namespace ExchangeModels
{
    public class Currency
    {
        [BsonElement("code")]
        public string Code { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }
    }
}