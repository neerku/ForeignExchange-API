using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForeignExchange.Models;
using ForeignExchange.Models.Projections;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ForeignExchange.Repositories
{
    public class CurrencyTSRepository
    {
        private readonly IMongoCollection<CurrencyTS> mongoCollection;
        private readonly IMongoClient mongoClient;

        public CurrencyTSRepository(IMongoClient client)
        {
            mongoClient = client;
            mongoCollection = mongoClient.GetDatabase(APIConstant.CurrencyDatabase)
                .GetCollection<Models.CurrencyTS>(APIConstant.CurrencyTSCollection);
        }

        public async Task<List<CurrencyCandlestick>> GetDataAsync(string fromAndTocurrency)
        {
           
            var matchStage = new BsonDocument("$match",
                new BsonDocument("symbol", fromAndTocurrency));

            //I limit the number of results
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
                                    { "unit", "minute" },
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


    }
}
