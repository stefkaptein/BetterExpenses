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
        var writeModels = new List<WriteModel<UserExpense>>();
        var accounts = await _monetaryAccountService.GetAccountsToAnalyse(task.UserId);
        foreach (var account in accounts)
        {
            writeModels.AddRange(await FetchExpenses(account.Id, task.UserId, task.FetchTill, account.FetchedTill));
        }

        await _monetaryAccountService.UpdateFetchedTill(accounts.ToDictionary(x => x.Id, _ => task.FetchTill));
        await _expensesMongoService.BulkWrite(writeModels);
        await _calculatorTaskService.DeleteTask<FetchExpensesTask>(task.Id);
        
        logger.LogInformation("Wrote {BulkWriteCount} write models for user {UserId}", writeModels.Count, task.UserId);
        return true;
    }

    private async Task<List<WriteModel<UserExpense>>> FetchExpenses(int accountId, Guid userId, DateTime fetchTill, DateTime fetchedTill)
    {
        var existing = await _expensesMongoService.GetExpensesForAccount(accountId);
        IEnumerable<UserExpense> expensesToAdd;
        
        if (existing.Count == 0 || fetchedTill == DateTime.MinValue)
        {
            expensesToAdd = _expensesApiService.GetExpenses(userId, accountId, fetchTill);
        } 
        else if (fetchedTill > fetchTill)
        {
            var oldestId = existing.Last().Id;
            expensesToAdd = _expensesApiService.GetExpensesFrom(userId, accountId, oldestId, fetchTill);
        }
        else
        {
            var newestId = existing.First().Id;
            expensesToAdd = _expensesApiService
                .GetExpensesAfter(userId, accountId, newestId)
                .OrderBy(x => x.Updated);
        }
        
        return expensesToAdd
            .Select(x => new InsertOneModel<UserExpense>(x))
            .ToList<WriteModel<UserExpense>>();
    }
}