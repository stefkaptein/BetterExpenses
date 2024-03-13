using BetterExpenses.CalculatorWorker.Models.Aggregations;
using BetterExpenses.Common.Database.Mongo;
using BetterExpenses.Common.Extensions;
using BetterExpenses.Common.Models.Expenses;
using BetterExpenses.Common.Services.MonetaryAccounts;
using BetterExpenses.Common.Services.Mongo.Expenses;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BetterExpenses.CalculatorWorker.Workers.Expenses.Processing.Graphs.RecurringExpenses;

public class RecurringExpensesGraphCreator(
    IMonetaryAccountService monetaryAccountService,
    IExpensesMongoService expensesMongoService,
    IFixedExpensesMongoService fixedExpensesMongoService,
    ILogger<RecurringExpensesGraphCreator> logger) : IGraphGenerator
{
    private readonly IExpensesMongoService _expensesMongoService = expensesMongoService;
    private readonly IFixedExpensesMongoService _fixedExpensesMongoService = fixedExpensesMongoService;

    private readonly IMonetaryAccountService _monetaryAccountService = monetaryAccountService;

    private readonly RecurringExpensesClassifier _recurringExpensesClassifier = new();

    public async Task Execute(Guid userId)
    {
        var accounts = await _monetaryAccountService.GetAccountsToAnalyse(userId);
        if (accounts.Count == 0)
        {
            logger.LogInformation("No accounts to analyse fixed expenses for user {UserId}", userId);
            return;
        }

        // TODO: Add someway to handle accounts that have been collect till different dates.
        var firstAccount = accounts.First();
        var monthsBack = DateTime.UtcNow.Date.GetNumberOfMonthsBack(firstAccount.FetchedTill.Date);
        var accountIds = accounts.Select(x => x.Id).ToArray();

        var cursor = await _expensesMongoService.Aggregate(Pipeline(accountIds, monthsBack));

        var fixedUserExpenses = new List<FixedUserExpense>();
        while (await cursor.MoveNextAsync() && cursor.Current != null)
        {
            fixedUserExpenses.AddRange(FindFixedExpenses(cursor.Current, userId));
        }

        await _fixedExpensesMongoService.InsertMany(fixedUserExpenses);
    }

    private IEnumerable<FixedUserExpense> FindFixedExpenses(
        IEnumerable<RecurringExpensesAggregation> aggregationsResults, Guid userId)
    {
        foreach (var aggRes in aggregationsResults)
        {
            if (!_recurringExpensesClassifier.IsFixedUserExpense(aggRes))
            {
                continue;
            }

            yield return _recurringExpensesClassifier.GetFixedUserExpense(aggRes, userId);
        }
    }

    private static PipelineDefinition<UserExpense, RecurringExpensesAggregation> Pipeline(
        IEnumerable<int> monetaryAccountIds, int monthsBack) =>
        new[]
        {
            new BsonDocument("$match",
                new BsonDocument
                {
                    {
                        "MonetaryAccountId",
                        new BsonDocument("$in",
                            new BsonArray(monetaryAccountIds))
                    },
                    { "Type", "BUNQ" },
                    { "SubType", "REQUEST" },
                    {
                        "Amount",
                        new BsonDocument("$lt", 0)
                    }
                }),
            new BsonDocument("$group",
                new BsonDocument
                {
                    {
                        "_id",
                        new BsonDocument
                        {
                            { "DisplayName", "$CounterpartyAlias.LabelMonetaryAccount.DisplayName" },
                            { "Iban", "$CounterpartyAlias.LabelMonetaryAccount.Iban" }
                        }
                    },
                    {
                        "Payments",
                        new BsonDocument("$addToSet", "$$ROOT")
                    },
                    {
                        "Dates",
                        new BsonDocument("$addToSet",
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
                                }
                            })
                    },
                    {
                        "NumberOfPayments",
                        new BsonDocument("$count",
                            new BsonDocument())
                    },
                    {
                        "Mean",
                        new BsonDocument("$avg", "$Amount")
                    },
                    {
                        "Min",
                        new BsonDocument("$min", "$Amount")
                    },
                    {
                        "Max",
                        new BsonDocument("$max", "$Amount")
                    }
                }),
            new BsonDocument("$match",
                new BsonDocument("NumberOfPayments",
                    new BsonDocument
                    {
                        { "$gte", 3 },
                        { "$lte", monthsBack + 1 }
                    })),
            new BsonDocument("$project",
                new BsonDocument
                {
                    { "_id", 1 },
                    { "Payments", 1 },
                    { "Dates", 1 },
                    { "NumberOfPayments", 1 },
                    { "Mean", 1 },
                    {
                        "DifferenceWithMean",
                        new BsonDocument("$add",
                            new BsonArray
                            {
                                new BsonDocument("$subtract",
                                    new BsonArray
                                    {
                                        "$Mean",
                                        "$Min"
                                    }),
                                new BsonDocument("$subtract",
                                    new BsonArray
                                    {
                                        "$Max",
                                        "$Mean"
                                    })
                            })
                    }
                })
        };
}