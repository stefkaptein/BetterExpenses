using BetterExpenses.Common.Database.Mongo;
using BetterExpenses.Common.Models.Expenses;
using BetterExpenses.Common.Models.Graphs;
using BetterExpenses.Common.Services.MonetaryAccounts;
using BetterExpenses.Common.Services.Mongo.Expenses;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BetterExpenses.CalculatorWorker.Workers.Expenses.Processing.Graphs;

public class MonthlyExpensesGraphCreator(
    IMonetaryAccountService monetaryAccountService,
    IExpensesMongoService expensesMongoService,
    IExpensesGraphMongoService expensesGraphMongoService,
    ILogger<MonthlyExpensesGraphCreator> logger) : IGraphGenerator
{
    private readonly IMonetaryAccountService _monetaryAccountService = monetaryAccountService;
    private readonly IExpensesMongoService _expensesMongoService = expensesMongoService;
    private readonly IExpensesGraphMongoService _expensesGraphMongoService = expensesGraphMongoService;

    public async Task Execute(Guid userId)
    {
        logger.LogInformation("Calculating monthly expenses graph for user {UserId}", userId);
        var accountIds = await _monetaryAccountService.GetAccountIdsToAnalyse(userId);
        if (accountIds.Count == 0)
        {
            logger.LogInformation("No accounts to analyse expenses for user {UserId}", userId);
            return;
        }

        var pipeline = Pipeline(accountIds);
        var aggregationResult = await (await _expensesMongoService.Aggregate(pipeline)).ToListAsync();
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
        await _expensesGraphMongoService.InsertOne(graph);
    }

    private static PipelineDefinition<UserExpense, DayExpensesAmountDataPoint> Pipeline(
        IEnumerable<int> monetaryAccountIds) =>
        new[]
        {
            new BsonDocument("$match",
                new BsonDocument("MonetaryAccountId",
                    new BsonDocument("$in",
                        new BsonArray(monetaryAccountIds)))),
            new BsonDocument("$match",
                new BsonDocument
                {
                    {
                        "Type",
                        new BsonDocument("$in",
                            new BsonArray
                            {
                                "MASTERCARD",
                                "BUNQ",
                                "IDEAL"
                            })
                    },
                    {
                        "SubType",
                        new BsonDocument("$in",
                            new BsonArray
                            {
                                "PAYMENT",
                                "REQUEST",
                                "BILLING"
                            })
                    },
                    {
                        "Amount",
                        new BsonDocument("$lt", 0)
                    }
                }),
            new BsonDocument("$project",
                new BsonDocument
                {
                    {
                        "Year",
                        new BsonDocument("$year", "$Updated")
                    },
                    {
                        "Month",
                        new BsonDocument("$month", "$Updated")
                    },
                    {
                        "Day",
                        new BsonDocument("$dayOfMonth", "$Updated")
                    },
                    { "Amount", "$Amount" }
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