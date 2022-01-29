using System;
using MongoDB.Driver;

namespace ForeignExchange.Repositories
{
    public class CurrencyRepository
    {
        private readonly IMongoCollection<Models.Currency> currencyCollection;
        private readonly IMongoClient mongoClient;

        public CurrencyRepository(IMongoClient client)
        {
            mongoClient = client;
            currencyCollection = mongoClient.GetDatabase(APIConstant.CurrencyDatabase)
                .GetCollection<Models.Currency>(APIConstant.CurrencyCollection);
        }
    }
}
