using BetterExpenses.CalculatorWorker.Workers.Expenses.Processing.Graphs;
using BetterExpenses.Common.Extensions;
using BetterExpenses.Common.Models.Tasks;
using BetterExpenses.Common.Services.Tasks;

namespace BetterExpenses.CalculatorWorker.Workers.Expenses.Processing;

public interface IProcessExpensesTaskRunner : ITaskRunner<ProcessExpensesTask>;

public class ProcessExpensesTaskRunner(IServiceScopeFactory serviceScopeFactory) : IProcessExpensesTaskRunner
{
    private readonly List<IGraphGenerator> _graphGenerators =
        serviceScopeFactory.GetRequiredServicesImplementingTypeInOwnScope<IGraphGenerator>();

    public async Task<bool> RunCycle(ProcessExpensesTask task)
    {
        await Task.WhenAll(_graphGenerators.Select(x => x.Execute(task.UserId)));
        return true;
    }
}