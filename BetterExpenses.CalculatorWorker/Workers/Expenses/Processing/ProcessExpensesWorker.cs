namespace BetterExpenses.CalculatorWorker.Workers.Expenses.Processing;

public class ProcessExpensesWorker(ILogger<ProcessExpensesWorker> logger, IServiceScopeFactory serviceScopeFactory)
    : Worker(serviceScopeFactory)
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var fetchExpenseTaskRunner = ServiceScope.ServiceProvider.GetRequiredService<IProcessExpensesTaskRunner>();
        logger.LogInformation("Process expenses worker is running");

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
                logger.LogError(e, "Process expenses worker threw exception");
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}