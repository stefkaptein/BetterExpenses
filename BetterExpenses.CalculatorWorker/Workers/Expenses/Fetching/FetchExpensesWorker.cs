namespace BetterExpenses.CalculatorWorker.Workers.Expenses.Fetching;

public class FetchExpensesWorker(ILogger<FetchExpensesWorker> logger, IServiceScopeFactory serviceScopeFactory)
    : Worker(serviceScopeFactory)
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var fetchExpenseTaskRunner = ServiceScope.ServiceProvider.GetRequiredService<IFetchExpensesTaskRunner>();
        logger.LogInformation("Fetch expenses worker is running");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (await fetchExpenseTaskRunner.RunCycle())
                {
                    continue;
                }
                logger.LogDebug("Nothing to process, waiting");
                await Task.Delay(5000, stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Fetch expenses worker threw exception");
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}