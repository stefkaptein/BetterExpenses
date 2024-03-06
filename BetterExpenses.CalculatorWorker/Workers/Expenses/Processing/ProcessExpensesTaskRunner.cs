using BetterExpenses.CalculatorWorker.Workers.Expenses.Processing.Graphs;
using BetterExpenses.Common.Models.Tasks;
using BetterExpenses.Common.Services.Tasks;

namespace BetterExpenses.CalculatorWorker.Workers.Expenses.Processing;

public interface IProcessExpensesTaskRunner
{
    public Task<bool> RunCycle();
}

public class ProcessExpensesTaskRunner(ICalculatorTaskService calculatorTaskService, MonthlyExpensesGraphCreator monthlyExpensesGraphCreator) : IProcessExpensesTaskRunner
{
    private readonly ICalculatorTaskService _calculatorTaskService = calculatorTaskService;
    private readonly MonthlyExpensesGraphCreator _monthlyExpensesGraphCreator = monthlyExpensesGraphCreator;

    public async Task<bool> RunCycle()
    {
        var task = await _calculatorTaskService.GetNextTask<ProcessExpensesTask>();
        if (task == null)
        {
            return false;
        }

        await _monthlyExpensesGraphCreator.Execute(task.UserId);
        await _calculatorTaskService.DeleteTask<ProcessExpensesTask>(task.Id);
        return true;
    }
}