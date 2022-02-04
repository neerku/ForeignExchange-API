using ExchangeDataGenerator.Generator;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace ForeignExchange.Repositories.Extension
{
    public static class ServiceExtension
    {
        public static MongoClient client;

        /// <summary>
        /// add data access services async
        /// </summary>
        /// <param name="services"></param>
        /// <param name="mongoUri"></param>
        public static void AddDataAccessServicesAsync(this IServiceCollection services, string mongoUri)
        {
            client = new MongoClient(mongoUri);
            services.AddSingleton<IMongoClient, MongoClient>(s =>
            {
                return client;
            });

            services.AddScoped<CurrencyRepository>();
            services.AddScoped<CurrencyTSRepository>();


            var generateBTC = new BTCCurrencyTimeSeries(client);
            Task.Run(() => generateBTC.GenerateBTCCurrencyData());
        }
    }
}