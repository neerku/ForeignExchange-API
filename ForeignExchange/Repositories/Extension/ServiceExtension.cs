using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeDataGenerator.Generator;
using ForeignExchange.SignalR;
using ForeignExchange.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace ForeignExchange.Repositories.Extension
{
    public static class ServiceExtension
    {
        public static MongoClient client;
        public static void AddDataAccessServicesAsync(this IServiceCollection services, string mongoUri)
        {
            client = new MongoClient(mongoUri);
            services.AddSingleton<IMongoClient, MongoClient>(s =>
            {
                return client;
            });

            services.AddScoped<CurrencyRepository>();
            services.AddScoped<CurrencyTSRepository>();
            services.AddScoped<SendData>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            var currencyTSRepository = serviceProvider.GetService<CurrencyTSRepository>();
            var abc= serviceProvider.GetService<SendData>();
            var generateBTC = new BTCCurrencyTimeSeries(client);
            
            Task.Run(() => generateBTC.GenerateBTCCurrencyData());
            //Task.Run(() => abc.StartSendingCandlestickDataToclients());

            //await generateBTC.GenerateBTCCurrencyData();
            //await sendDataToClient.StartSendingCandlestickDataToclients();






        }

        //public static async Task AddDataAccessServicesAsync(this IServiceCollection services, string mongoUri)
        //{
        //    client = new MongoClient(mongoUri);
        //    services.AddSingleton<IMongoClient, MongoClient>(s =>
        //    {
        //        return client;
        //    });

        //    services.AddSingleton<CurrencyRepository>();
        //    services.AddSingleton<CurrencyTSRepository>();


        //    ServiceProvider serviceProvider = services.BuildServiceProvider();
        //    var currencyTSRepository = serviceProvider.GetService<CurrencyTSRepository>();
        //    var generateBTC = new BTCCurrencyTimeSeries(client);
        //    var sendDataToClient = new SendData(currencyTSRepository);

        //    Task.Run(() => generateBTC.GenerateBTCCurrencyData());
        //    Task.Run(() => generateBTC.GenerateBTCCurrencyData());

        //    //await generateBTC.GenerateBTCCurrencyData();
        //    //await sendDataToClient.StartSendingCandlestickDataToclients();






        //}

    }
}
