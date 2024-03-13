using BetterExpenses.Common.Database.Mongo;
using BetterExpenses.Common.Models.Expenses;
using BetterExpenses.Common.Models.Graphs;
using BetterExpenses.Common.Services.MonetaryAccounts;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BetterExpenses.CalculatorWorker.Workers.Expenses.Processing.Graphs;

public class MonthlyExpensesGraphCreator(IMongoConnection mongoConnection, 
    IMonetaryAccountService monetaryAccountService, 
    ILogger<MonthlyExpensesGraphCreator> logger)
{
    private readonly IMongoCollection<UserExpense> _sourceCollection =
        mongoConnection.GetCollection<UserExpense>(MongoConnection.ExpensesCollectionName);

    private readonly IMongoCollection<ExpensesGraph> _targetCollection =
        mongoConnection.GetCollection<ExpensesGraph>(MongoConnection.ExpensesGraphsCollectionName);

    private readonly IMonetaryAccountService _monetaryAccountService = monetaryAccountService;

    public async Task Execute(Guid userId)
    {
        logger.LogInformation("Calculating monthly expenses graph for user {UserId}", userId);
        var accountIds = await _monetaryAccountService.GetAccountIdsToAnalyse(userId);
        
        var pipeline = Pipeline(accountIds);
        var aggregationResult = await _sourceCollection.Aggregate(pipeline).ToListAsync();
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
        await _targetCollection.InsertOneAsync(graph);
    }

    private static PipelineDefinition<UserExpense, DayExpensesAmountDataPoint> Pipeline(IEnumerable<int> monetaryAccountIds) =>
        new[]
        {
            new BsonDocument("$match", 
                new BsonDocument("MonetaryAccountId", 
                    new BsonDocument("$in", 
                        new BsonArray(monetaryAccountIds)))),
            new BsonDocument("$match", 
                new BsonDocument
                {
                    { "Type", 
                        new BsonDocument("$in", 
                            new BsonArray
                            {
                                "MASTERCARD",
                                "BUNQ",
                                "IDEAL"
                            }) }, 
                    { "SubType", 
                        new BsonDocument("$in", 
                            new BsonArray
                            {
                                "PAYMENT",
                                "REQUEST",
                                "BILLING"
                            }) }, 
                    { "Amount", 
                        new BsonDocument("$lt", 0) }
                }),
            new BsonDocument("$project", 
                new BsonDocument
                {
                    { "Year", 
                        new BsonDocument("$year", "$Updated") }, 
                    { "Month", 
                        new BsonDocument("$month", "$Updated") }, 
                    { "Day", 
                        new BsonDocument("$dayOfMonth", "$Updated") }, 
                    { "Amount", "$Amount" }
                }),
            new BsonDocument("$group", 
                new BsonDocument
                {
                    { "_id", 
                        new BsonDocument
                        {
                            { "Year", "$Year" }, 
                            { "Month", "$Month" }, 
                            { "Day", "$Day" }
                        } }, 
                    { "Amount", 
                        new BsonDocument("$sum", "$Amount") }
                })
        };
}