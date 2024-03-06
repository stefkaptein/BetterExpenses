using BetterExpenses.Common.Models.Expenses;
using BetterExpenses.Common.Models.Tasks;
using BetterExpenses.Common.Services.Bunq;
using BetterExpenses.Common.Services.Expenses;
using BetterExpenses.Common.Services.MonetaryAccounts;
using BetterExpenses.Common.Services.Tasks;
using MongoDB.Driver;

namespace BetterExpenses.CalculatorWorker.Workers.Expenses.Fetching;

public interface IFetchExpensesTaskRunner
{
    public Task<bool> RunCycle();
}

public class FetchExpensesTaskRunner(
    ICalculatorTaskService calculatorTaskService,
    IBunqExpensesService expensesApiService,
    IMonetaryAccountService monetaryAccountService,
    IExpensesMongoService expensesMongoService,
    ILogger<FetchExpensesTaskRunner> logger) : IFetchExpensesTaskRunner
{
    private readonly ICalculatorTaskService _calculatorTaskService = calculatorTaskService;
    private readonly IBunqExpensesService _expensesApiService = expensesApiService;
    private readonly IMonetaryAccountService _monetaryAccountService = monetaryAccountService;
    private readonly IExpensesMongoService _expensesMongoService = expensesMongoService;

    public async Task<bool> RunCycle()
    {
        var task = await _calculatorTaskService.GetNextTask<FetchExpensesTask>();
        if (task == null)
        {
            return false;
        }

        logger.LogInformation("Fetching expenses for user {UserId}", task.UserId);
        var writeModels = new List<WriteModel<UserAccountExpensesList>>();
        var accountIds = await _monetaryAccountService.GetAccountIdsToAnalyse(task.UserId);
        foreach (var accountId in accountIds)
        {
            writeModels.Add(await FetchExpenses(accountId, task.UserId, task.FetchTill));
        }

        await _expensesMongoService.BulkWrite(writeModels);
        await _calculatorTaskService.DeleteTask<FetchExpensesTask>(task.Id);
        
        logger.LogInformation("Wrote {BulkWriteCount} write models for user {UserId}", writeModels.Count, task.UserId);
        return true;
    }

    private async Task<WriteModel<UserAccountExpensesList>> FetchExpenses(int accountId, Guid userId, DateTime fetchTill)
    {
        var existing = await _expensesMongoService.FindFirst(x => x.UserId == userId && x.AccountId == accountId);
        if (existing == null || existing.Expenses.Count == 0)
        {
            var list = _expensesApiService.GetExpenses(userId, accountId, fetchTill);
            return new InsertOneModel<UserAccountExpensesList>(new UserAccountExpensesList
            {
                AccountId = accountId,
                UserId = userId,
                Expenses = list,
                FetchedUntil = fetchTill
            });
        }
        
        if (existing.FetchedUntil != fetchTill)
        {
            var oldestId = existing.Expenses.Last().Id;
            var list = _expensesApiService.GetExpensesFrom(userId, accountId, oldestId, fetchTill);
            return new UpdateOneModel<UserAccountExpensesList>(
                Builders<UserAccountExpensesList>.Filter.Eq(x => x.Id, existing.Id),
                Builders<UserAccountExpensesList>.Update.PushEach(x => x.Expenses, list)
            );
        }
        else
        {
            var newestId = existing.Expenses.First().Id;
            var list = _expensesApiService
                .GetExpensesAfter(userId, accountId, newestId)
                .OrderBy(x => x.Updated);
            
            return new UpdateOneModel<UserAccountExpensesList>(
                Builders<UserAccountExpensesList>.Filter.Eq(x => x.Id, existing.Id),
                Builders<UserAccountExpensesList>.Update.PushEach(x => x.Expenses, list, position: 0)
            );
        }
    }
}