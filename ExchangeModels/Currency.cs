﻿using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ExchangeModels
{
    public class Currency
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("code")]
        public string Code { get; set; }
    }
}