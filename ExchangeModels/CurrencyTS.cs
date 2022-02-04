using MongoDB.Bson.Serialization.Attributes;
using System;

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

        [BsonElement("price")]
        public double Price { get; set; }

        [BsonElement("symbol")]
        public string Symbol { get; set; }

        [BsonElement("time")]
        public DateTime Time { get; set; }


    }
}