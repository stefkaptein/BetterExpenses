namespace BetterExpenses.CalculatorWorker.Workers.Expenses.Processing;

public class ProcessExpensesWorker(ILogger<ProcessExpensesWorker> logger, IServiceScopeFactory serviceScopeFactory)
    : Worker<ProcessExpensesWorker>(serviceScopeFactory, logger)
{
    private IProcessExpensesTaskRunner _fetchExpensesTaskRunner = null!;

    protected override string WorkerName => "Process expenses worker";

    protected override void InitServices()
    {
        _fetchExpensesTaskRunner = ServiceScope.ServiceProvider.GetRequiredService<IProcessExpensesTaskRunner>();
    }

    protected override async Task<bool> RunCycle()
    {
        return await _fetchExpensesTaskRunner.RunCycle();
    }
}