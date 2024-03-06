using BetterExpenses.Common.Database.Mongo;
using BetterExpenses.Common.Models.Expenses;
using BetterExpenses.Common.Models.Graphs;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BetterExpenses.CalculatorWorker.Workers.Expenses.Processing.Graphs;

public class MonthlyExpensesGraphCreator(IMongoConnection mongoConnection, ILogger<MonthlyExpensesGraphCreator> logger)
{
    public async Task Execute(Guid userId)
    {
        logger.LogInformation("Calculating monthly expenses graph for user {UserId}", userId);

        var sourceCollection =
            mongoConnection.GetCollection<UserAccountExpensesList>(MongoConnection.ExpensesCollectionName);
        var targetCollection =
            mongoConnection.GetCollection<ExpensesGraph>(MongoConnection.ExpensesGraphsCollectionName);

        var pipeline = Pipeline(userId);
        var aggregationResult = await sourceCollection.Aggregate(pipeline).ToListAsync();
        if (aggregationResult == null)
        {
            logger.LogWarning("Monthly expenses aggregation for user {UserId} result returned null", userId);
            return;
        }

        var graph = new ExpensesGraph
        {
            UserId = userId,
            DataPoints = aggregationResult
        };
        
        logger.LogInformation("Inserting monthly expenses graph for user {UserId}", userId);
        await targetCollection.InsertOneAsync(graph);
    }

    private static PipelineDefinition<UserAccountExpensesList, DayExpensesAmountDataPoint> Pipeline(Guid userId) => new[]
    {   
        new BsonDocument("$match", 
            new BsonDocument("UserId", new BsonBinaryData(userId, GuidRepresentation.Standard))),
        new BsonDocument("$unwind", "$Expenses"),
        new BsonDocument("$match",
            new BsonDocument
            {
                {
                    "Expenses.Type",
                    new BsonDocument("$in",
                        new BsonArray
                        {
                            "MASTERCARD",
                            "BUNQ",
                            "IDEAL"
                        })
                },
                {
                    "Expenses.SubType",
                    new BsonDocument("$in",
                        new BsonArray
                        {
                            "PAYMENT",
                            "REQUEST",
                            "BILLING"
                        })
                },
                {
                    "Expenses.Amount",
                    new BsonDocument("$lt", 0)
                }
            }),
        new BsonDocument("$project",
            new BsonDocument
            {
                {
                    "Year",
                    new BsonDocument("$year", "$Expenses.Updated")
                },
                {
                    "Month",
                    new BsonDocument("$month", "$Expenses.Updated")
                },
                {
                    "Day",
                    new BsonDocument("$dayOfMonth", "$Expenses.Updated")
                },
                { "Amount", "$Expenses.Amount" }
            }),
        new BsonDocument("$group",
            new BsonDocument
            {
                {
                    "_id",
                    new BsonDocument
                    {
                        { "Year", "$Year" },
                        { "Month", "$Month" },
                        { "Day", "$Day" }
                    }
                },
                {
                    "Amount",
                    new BsonDocument("$sum", "$Amount")
                }
            })
    };
}