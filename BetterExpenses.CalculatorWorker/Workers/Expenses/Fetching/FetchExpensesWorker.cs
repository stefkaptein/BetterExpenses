namespace BetterExpenses.CalculatorWorker.Workers.Expenses.Fetching;

public class FetchExpensesWorker(ILogger<FetchExpensesWorker> logger, IServiceScopeFactory serviceScopeFactory)
    : Worker<FetchExpensesWorker>(serviceScopeFactory, logger)
{
    private IFetchExpensesTaskRunner _fetchExpensesTaskRunner = null!;
    
    protected override string WorkerName => "Fetch expenses worker";

    protected override void InitServices()
    {
        _fetchExpensesTaskRunner = ServiceScope.ServiceProvider.GetRequiredService<IFetchExpensesTaskRunner>();
    }

    protected override async Task<bool> RunCycle()
    {
        return await _fetchExpensesTaskRunner.RunCycle();
    }
}