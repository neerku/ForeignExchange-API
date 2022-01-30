using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeModels;
using ExchangeModels.Projections;
using ForeignExchange.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ForeignExchange.Repositories
{
    public class CurrencyTSRepository
    {
        private readonly IMongoCollection<CurrencyTS> mongoCollection;
        private readonly IMongoClient mongoClient;
        private readonly IHubContext<CurrencyCandlestickHub> _hub;

       
        public CurrencyTSRepository(IMongoClient client, IHubContext<CurrencyCandlestickHub> hub)
        {
            mongoClient = client;
            mongoCollection = mongoClient.GetDatabase(APIConstant.CurrencyDatabase)
                .GetCollection<CurrencyTS>(APIConstant.CurrencyTSCollection);
            _hub = hub;
        }

        public async Task<List<CurrencyCandlestick>> GetDataAsync(string fromAndTocurrency)
        {
           
            var matchStage = new BsonDocument("$match",
                new BsonDocument("symbol", fromAndTocurrency));

            var groupStage = new BsonDocument("$group",
                new BsonDocument
                    {
                            { "_id",
                new BsonDocument
                        {
                            { "symbol", "$symbol" },
                            { "time",
                new BsonDocument("$dateTrunc",
                new BsonDocument
                                {
                                    { "date", "$time" },
                                    { "unit", "second" },
                                    { "binSize", 5 }
                                }) }
                        } },
                        { "high",
                new BsonDocument("$max", "$price") },
                        { "low",
                new BsonDocument("$min", "$price") },
                        { "open",
                new BsonDocument("$first", "$price") },
                        { "close",
                new BsonDocument("$last", "$price") }
                    });

            var sortStage = new BsonDocument("$sort",
                new BsonDocument("_id.time", -1));

            var pipeline = new[]
            {
                matchStage,
                groupStage,
                sortStage

            };

            var result = await mongoCollection
                .Aggregate(PipelineDefinition<CurrencyTS, CurrencyCandlestick>.Create(pipeline))
                .ToListAsync();


            return result;

        }

        public async Task<List<CurrencyCandlestick>> GetDataAsync(string[] currencyList)
        {
            var currencyArray = new BsonArray();
            currencyArray.AddRange(currencyList);

            var matchStage = new BsonDocument("$match",
                new BsonDocument("symbol", new BsonDocument("$in",currencyArray)));

            var groupStage = new BsonDocument("$group",
                new BsonDocument
                    {
                            { "_id",
                new BsonDocument
                        {
                            { "symbol", "$symbol" },
                            { "time",
                new BsonDocument("$dateTrunc",
                new BsonDocument
                                {
                                    { "date", "$time" },
                                    { "unit", "second" },
                                    { "binSize", 10 }
                                }) }
                        } },
                        { "high",
                new BsonDocument("$max", "$price") },
                        { "low",
                new BsonDocument("$min", "$price") },
                        { "open",
                new BsonDocument("$first", "$price") },
                        { "close",
                new BsonDocument("$last", "$price") }
                    });

            var sortStage = new BsonDocument("$sort",
                new BsonDocument("_id.time", -1));

            var pipeline = new[]
            {
                matchStage,
                groupStage,
                sortStage

            };

            var result = await mongoCollection
                .Aggregate(PipelineDefinition<CurrencyTS, CurrencyCandlestick>.Create(pipeline))
                .ToListAsync();


            return result;

        }

        public async Task SendCandlestickDataToclients()
        {
            var data = await this.GetDataAsync(Constants.CurrencySymbol);
            await _hub.Clients.All.SendAsync("transferredData", data[0]);
           
        }

    }
}
