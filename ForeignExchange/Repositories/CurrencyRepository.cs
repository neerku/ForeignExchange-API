using System;
using ExchangeModels;
using MongoDB.Driver;

namespace ForeignExchange.Repositories
{
    public class CurrencyRepository
    {
        private readonly IMongoCollection<Currency> currencyCollection;
        private readonly IMongoClient mongoClient;

        public CurrencyRepository(IMongoClient client)
        {
            mongoClient = client;
            currencyCollection = mongoClient.GetDatabase(APIConstant.CurrencyDatabase)
                .GetCollection<Currency>(APIConstant.CurrencyCollection);
        }
    }
}
