using ExchangeModels;
using ExchangeModels.Projections;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForeignExchange.Repositories
{
    public class CurrencyTSRepository
    {
        private readonly IMongoClient mongoClient;
        private readonly IMongoCollection<CurrencyTS> mongoCollection;

        public CurrencyTSRepository(IMongoClient client)
        {
            mongoClient = client;
            mongoCollection = mongoClient.GetDatabase(APIConstant.CurrencyDatabase)
                .GetCollection<CurrencyTS>(APIConstant.CurrencyTSCollection);
        }

        public async Task<List<CurrencyCandlestick>> GetCandleDataAsync(string[] currencyList)
        {
            var currencyArray = new BsonArray();
            currencyArray.AddRange(currencyList);

            var matchStage = new BsonDocument("$match",
            new BsonDocument
                {
                    { "symbol", "BTC-USD" },
                    { "time",
            new BsonDocument("$gte",
            new DateTime(2022, 1, 30, 12, 30, 46)) }
                });

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
                sortStage,
                new BsonDocument("$limit", 500)
            };

            var result = await mongoCollection
                .Aggregate(PipelineDefinition<CurrencyTS, CurrencyCandlestick>.Create(pipeline))
                .ToListAsync();

            return result;
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

        public async Task<List<CurrencyEMA>> GetEMADataAsync(string[] currencyList)
        {
            var currencyArray = new BsonArray();
            currencyArray.AddRange(currencyList);

            var matchStage = new BsonDocument("$match",
    new BsonDocument
        {
            { "symbol",
    new BsonDocument("$in",currencyArray) },
            { "time",
    new BsonDocument("$gte",
    new DateTime(2022, 1, 30, 12, 30, 46)) }
        });

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

            var projectStage = new BsonDocument("$project",
                            new BsonDocument
                                {
                                    { "_id", 1 },
                                    { "price", "$close" }
                                });

            var setWStage = new BsonDocument("$setWindowFields",
                            new BsonDocument
                                {
                                    { "partitionBy", "_id.symbol" },
                                    { "sortBy",
                            new BsonDocument("_id.time", 1) },
                                    { "output",
                            new BsonDocument
                                    {
                                        { "sma_3",
                            new BsonDocument
                                        {
                                            { "$avg", "$price" },
                                            { "window",
                            new BsonDocument("documents",
                            new BsonArray
                                                {
                                                    -3,
                                                    0
                                                }) }
                                        } },
                                        { "sma_10",
                            new BsonDocument
                                        {
                                            { "$avg", "$price" },
                                            { "window",
                            new BsonDocument("documents",
                            new BsonArray
                                                {
                                                    -10,
                                                    0
                                                }) }
                                        } },
                                        { "ema_5",
                            new BsonDocument("$expMovingAvg",
                            new BsonDocument
                                            {
                                                { "input", "$price" },
                                                { "N", 5 }
                                            }) },
                                        { "ema_10",
                            new BsonDocument("$expMovingAvg",
                            new BsonDocument
                                            {
                                                { "input", "$price" },
                                                { "N", 10 }
                                            }) }
                                    } }
                                });

            var setStage = new BsonDocument("$set",
    new BsonDocument
        {
            { "sma_3",
    new BsonDocument("$round",
    new BsonArray
                {
                    "$sma_3",
                    2
                }) },
            { "sma_10",
    new BsonDocument("$round",
    new BsonArray
                {
                    "$sma_10",
                    2
                }) },
            { "ema_5",
    new BsonDocument("$round",
    new BsonArray
                {
                    "$ema_5",
                    2
                }) },
            { "ema_10",
    new BsonDocument("$round",
    new BsonArray
                {
                    "$ema_10",
                    2
                }) }
        });

            var pipeline = new[]
            {
                matchStage,
                groupStage,
                sortStage,
                projectStage,
                setWStage,
                setStage
            };

            var result = await mongoCollection
                .Aggregate(PipelineDefinition<CurrencyTS, CurrencyEMA>.Create(pipeline))
                .ToListAsync();

            return result;
        }

        public async Task<List<CurrencyEMA>> GetMinMaxDataAsync(string[] currencyList)
        {
            var currencyArray = new BsonArray();
            currencyArray.AddRange(currencyList);

            var matchStage = new BsonDocument("$match",
    new BsonDocument
        {
            { "symbol",
    new BsonDocument("$in",currencyArray) },
            { "time",
    new BsonDocument("$gte",
    new DateTime(2022, 1, 30, 12, 30, 46)) }
        });

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

            var projectStage = new BsonDocument("$project",
                            new BsonDocument
                                {
                                    { "_id", 1 },
                                    { "price", "$close" }
                                });

            var setWStage = new BsonDocument("$setWindowFields",
                            new BsonDocument
                                {
                                    { "partitionBy", "_id.symbol" },
                                    { "sortBy",
                            new BsonDocument("_id.time", 1) },
                                    { "output",
                            new BsonDocument
                                    {
                                        { "sma_3",
                            new BsonDocument
                                        {
                                            { "$avg", "$price" },
                                            { "window",
                            new BsonDocument("documents",
                            new BsonArray
                                                {
                                                    -3,
                                                    0
                                                }) }
                                        } },
                                        { "sma_10",
                            new BsonDocument
                                        {
                                            { "$avg", "$price" },
                                            { "window",
                            new BsonDocument("documents",
                            new BsonArray
                                                {
                                                    -10,
                                                    0
                                                }) }
                                        } },
                                        { "ema_5",
                            new BsonDocument("$expMovingAvg",
                            new BsonDocument
                                            {
                                                { "input", "$price" },
                                                { "N", 5 }
                                            }) },
                                        { "ema_10",
                            new BsonDocument("$expMovingAvg",
                            new BsonDocument
                                            {
                                                { "input", "$price" },
                                                { "N", 10 }
                                            }) }
                                    } }
                                });

            var setStage = new BsonDocument("$set",
    new BsonDocument
        {
            { "sma_3",
    new BsonDocument("$round",
    new BsonArray
                {
                    "$sma_3",
                    2
                }) },
            { "sma_10",
    new BsonDocument("$round",
    new BsonArray
                {
                    "$sma_10",
                    2
                }) },
            { "ema_5",
    new BsonDocument("$round",
    new BsonArray
                {
                    "$ema_5",
                    2
                }) },
            { "ema_10",
    new BsonDocument("$round",
    new BsonArray
                {
                    "$ema_10",
                    2
                }) }
        });

            var pipeline = new[]
            {
                matchStage,
                groupStage,
                sortStage,
                projectStage,
                setWStage,
                setStage,
                new BsonDocument("$limit", 500)
            };

            var result = await mongoCollection
                .Aggregate(PipelineDefinition<CurrencyTS, CurrencyEMA>.Create(pipeline))
                .ToListAsync();

            return result;
        }

        public async Task<List<CurrencyEMA>> GetDataAsync(string[] currencyList)
        {
            var currencyArray = new BsonArray();
            currencyArray.AddRange(currencyList);

            var matchStage = new BsonDocument("$match",
    new BsonDocument
        {
            { "symbol", "BTC-USD" },
            { "time",
    new BsonDocument("$gte",
    new DateTime(2022, 1, 30, 12, 30, 46)) }
        });

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

            var projectStage = new BsonDocument("$project",
                            new BsonDocument
                                {
                                    { "_id", 1 },
                                    { "price", "$close" }
                                });

            var setWStage = new BsonDocument("$setWindowFields",
                            new BsonDocument
                                {
                                    { "partitionBy", "_id.symbol" },
                                    { "sortBy",
                            new BsonDocument("_id.time", 1) },
                                    { "output",
                            new BsonDocument
                                    {
                                        { "sma_3",
                            new BsonDocument
                                        {
                                            { "$avg", "$price" },
                                            { "window",
                            new BsonDocument("documents",
                            new BsonArray
                                                {
                                                    -3,
                                                    0
                                                }) }
                                        } },
                                        { "sma_10",
                            new BsonDocument
                                        {
                                            { "$avg", "$price" },
                                            { "window",
                            new BsonDocument("documents",
                            new BsonArray
                                                {
                                                    -10,
                                                    0
                                                }) }
                                        } },
                                        { "ema_5",
                            new BsonDocument("$expMovingAvg",
                            new BsonDocument
                                            {
                                                { "input", "$price" },
                                                { "N", 5 }
                                            }) },
                                        { "ema_10",
                            new BsonDocument("$expMovingAvg",
                            new BsonDocument
                                            {
                                                { "input", "$price" },
                                                { "N", 10 }
                                            }) }
                                    } }
                                });

            var setStage = new BsonDocument("$set",
    new BsonDocument
        {
            { "sma_3",
    new BsonDocument("$round",
    new BsonArray
                {
                    "$sma_3",
                    2
                }) },
            { "sma_10",
    new BsonDocument("$round",
    new BsonArray
                {
                    "$sma_10",
                    2
                }) },
            { "ema_5",
    new BsonDocument("$round",
    new BsonArray
                {
                    "$ema_5",
                    2
                }) },
            { "ema_10",
    new BsonDocument("$round",
    new BsonArray
                {
                    "$ema_10",
                    2
                }) }
        });

            var pipeline = new[]
            {
                matchStage,
                groupStage,
                sortStage,
                projectStage,
                setWStage,
                setStage,
                new BsonDocument("$limit", 500)
            };

            var result = await mongoCollection
                .Aggregate(PipelineDefinition<CurrencyTS, CurrencyEMA>.Create(pipeline))
                .ToListAsync();

            return result;
        }
    }
}