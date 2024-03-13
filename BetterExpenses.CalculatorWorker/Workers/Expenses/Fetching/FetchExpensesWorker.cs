using BetterExpenses.Common.Models.Tasks;

namespace BetterExpenses.CalculatorWorker.Workers.Expenses.Fetching;

public class FetchExpensesWorker(ILogger<FetchExpensesWorker> logger, IServiceScopeFactory serviceScopeFactory)
    : Worker<FetchExpensesWorker, FetchExpensesTask>(serviceScopeFactory, logger)
{
    private IFetchExpensesTaskRunner _fetchExpensesTaskRunner = null!;
    
    protected override string WorkerName => "Fetch expenses worker";

    protected override void InitServices()
    {
        base.InitServices();
        _fetchExpensesTaskRunner = ServiceScope.ServiceProvider.GetRequiredService<IFetchExpensesTaskRunner>();
    }

    protected override async Task<bool> RunCycle(FetchExpensesTask task)
    {
        return await _fetchExpensesTaskRunner.RunCycle(task);
    }
}