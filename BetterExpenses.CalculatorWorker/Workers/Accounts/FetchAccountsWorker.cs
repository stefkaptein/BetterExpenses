namespace BetterExpenses.CalculatorWorker.Workers.Accounts;

public class FetchAccountsWorker(ILogger<FetchAccountsWorker> logger, IServiceScopeFactory serviceScopeFactory)
    : Worker(serviceScopeFactory)
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var fetchExpenseTaskRunner = ServiceScope.ServiceProvider.GetRequiredService<IFetchAccountsTaskRunner>();
        logger.LogInformation("Fetch account worker is running");
        
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
                logger.LogError(e, "Fetch accounts worker threw exception");
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}