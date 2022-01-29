using System;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace ForeignExchange.Repositories.Extension
{
    public static class ServiceExtension
    {
        public static MongoClient client;
        public static void AddDataAccessServices(this IServiceCollection services, string mongoUri)
        {
            client = new MongoClient(mongoUri);
            services.AddSingleton<IMongoClient, MongoClient>(s =>
            {
                return client;
            });

            services.AddSingleton<CurrencyRepository>();
            services.AddSingleton<CurrencyTSRepository>();

            
        }
    }
}
