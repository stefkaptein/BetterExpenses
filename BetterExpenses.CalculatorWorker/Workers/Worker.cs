namespace BetterExpenses.CalculatorWorker.Workers;

public abstract class Worker<T>(IServiceScopeFactory serviceScopeFactory, ILogger<T> logger) : BackgroundService
{
    protected readonly IServiceScope ServiceScope = serviceScopeFactory.CreateScope();

    protected abstract string WorkerName { get; }

    /// <summary>
    /// First called when starting the worker. Use the ServiceScope to get and initialize the required services.
    /// </summary>
    protected abstract void InitServices();
    
    /// <summary>
    /// Run the worker cycle, returns whether any work was done.
    ///
    /// When no work was done the worker pauses for longer to prevent too many empty loops being executed.
    /// </summary>
    /// <returns>Whether any work was performed.</returns>
    protected abstract Task<bool> RunCycle();
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        InitServices();
        
        logger.LogInformation("{WorkerName} is running", WorkerName);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (await RunCycle())
                {
                    continue;
                }
                logger.LogDebug("Nothing to process, waiting");
                await Task.Delay(5000, stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "{WorkerName} threw exception; {Message}", WorkerName, e.Message);
                await Task.Delay(10000, stoppingToken);
            }
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        ServiceScope.Dispose();
    }
}