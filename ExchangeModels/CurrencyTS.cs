using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ExchangeModels
{
    public class CurrencyTS
    {
        private string _id;

        [BsonElement("_id")]
        [BsonId]
        public string Name
        {
            get { return this._id; }
            set { this._id = value; }
        }

        [BsonElement("time")]
        public DateTime Time { get; set; }

        [BsonElement("symbol")]
        public string Symbol { get; set; }

        [BsonElement("price")]
        public double Price { get; set; }
    }
}
