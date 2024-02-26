using BetterExpenses.Common.Models.Expenses;
using BetterExpenses.Common.Models.Tasks;
using BetterExpenses.Common.Models.User;
using BetterExpenses.Common.Services.Bunq;
using BetterExpenses.Common.Services.Expenses;
using BetterExpenses.Common.Services.MonetaryAccounts;
using BetterExpenses.Common.Services.Tasks;
using Bunq.Sdk.Model.Generated.Endpoint;

namespace BetterExpenses.CalculatorWorker.Workers.Expenses;

public interface IFetchExpensesTaskRunner
{
    public Task<bool> RunCycle();
}

public class FetchExpensesTaskRunner(
    ICalculatorTaskService calculatorTaskService,
    IBunqExpensesService expensesApiService,
    IMonetaryAccountService monetaryAccountService,
    IExpensesMongoService expensesMongoService) : IFetchExpensesTaskRunner
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

        var userAccountExpenseLists = new List<UserAccountExpensesList>();
        var accountIds = await _monetaryAccountService.GetAccountIdsToAnalyse(task.UserId);
        foreach (var accountId in accountIds)
        {
            var list = _expensesApiService.GetExpenses(task.UserId, accountId, task.FetchTill);
            userAccountExpenseLists.Add(
                new UserAccountExpensesList
                {
                    AccountId = accountId,
                    UserId = task.UserId,
                    Expenses = list
                }
            );
        }
        
        await _expensesMongoService.InsertMany(userAccountExpenseLists);
        await _calculatorTaskService.DeleteTask<FetchExpensesTask>(task.Id);
        return true;
    }
}