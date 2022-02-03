using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ExchangeModels;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ExchangeDataGenerator.Generator
{
    public class BTCCurrencyTimeSeries
    {
        
        private readonly IMongoCollection<BsonDocument> currencyCollection;
        private readonly IMongoClient mongoClient;

        public BTCCurrencyTimeSeries(IMongoClient client)
        {
            mongoClient = client;
            currencyCollection = mongoClient.GetDatabase(Constants.CurrencyDatabase)
                .GetCollection<BsonDocument>(Constants.CurrencyCollection);
        }

        public async Task GenerateBTCCurrencyData()
        {
            try
            {
                var random = new Random();
                while (true)
                {
                    List<BsonDocument> sampleBsonList = new List<BsonDocument>();
                    foreach (var symbol in Constants.CurrencySymbol)
                    {
                        var document = new BsonDocument();
                        document.SetElement(new BsonElement("time", DateTime.UtcNow));
                        document.SetElement(new BsonElement("symbol", symbol));
                        double price;
                        switch (symbol)
                        {
                            case "BTC-USD":
                                price = GetRandomDouble(random, 36000.5463, 37000.00);
                                break;
                            case "BTC-GBP":
                                price = GetRandomDouble(random, 34000.00, 35000.00);
                                break;
                            case "BTC-INR":
                                price = GetRandomDouble(random, 290000.00, 300000.00);
                                break;
                            case "BTC-KYD":
                                price = GetRandomDouble(random, 39000.00, 40000.00);
                                break;
                            case "BTC-AUD":
                                price = GetRandomDouble(random, 69000.00, 70000.00);
                                break;
                            case "BTC-CNY":
                                price = GetRandomDouble(random, 290000.00, 300000.00);
                                break;
                            case "BTC-NZD":
                                price = GetRandomDouble(random, 64154.00, 65000.00);
                                break;
                            case "BTC-EUR":
                                price = GetRandomDouble(random, 39000.00, 40000.00);
                                break;
                            case "BTC-SGD":
                                price = GetRandomDouble(random, 59000.00, 60000.00);
                                break;
                            case "BTC-BRL":
                                price = GetRandomDouble(random, 241000.00, 250000.00);
                                break;
                            default:
                                price = GetRandomDouble(random, 241000.00, 250000.00);
                                break;
                        }
                        document.SetElement(new BsonElement("price", price));
                        sampleBsonList.Add(document);
                    }

                    await currencyCollection.InsertManyAsync(sampleBsonList);

                    // Insert data every two seconds
                    Thread.Sleep(2000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
        }

        public static double GetRandomDouble(Random random, double minimum, double maximum)
        {
            var randomDouble = random.NextDouble() * (maximum - minimum) + minimum;
            var roundedDouble = Math.Round(randomDouble, 4, MidpointRounding.AwayFromZero);
            return roundedDouble;
        }

        
    }
}
