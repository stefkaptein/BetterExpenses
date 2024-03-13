using BetterExpenses.Common.Models.Tasks;
using BetterExpenses.Common.Services.Tasks;

namespace BetterExpenses.CalculatorWorker.Workers;

public abstract class Worker<TWorker, TTask>(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<TWorker> logger)  : BackgroundService 
    where TTask : CalculatorTask
{
    private const int NoTaskTimeout = 5000;
    
    protected readonly IServiceScope ServiceScope = serviceScopeFactory.CreateScope();
    protected ICalculatorTaskService CalculatorTaskService = null!;
    
    protected abstract string WorkerName { get; }

    /// <summary>
    /// First called when starting the worker. Use the ServiceScope to get and initialize the required services.
    /// </summary>
    protected virtual void InitServices()
    {
        CalculatorTaskService = ServiceScope.ServiceProvider.GetRequiredService<ICalculatorTaskService>();
    }

    /// <summary>
    /// Run the worker cycle, returns whether any work was done.
    ///
    /// When no work was done the worker pauses for longer to prevent too many empty loops being executed.
    /// </summary>
    /// <returns>Whether any work was performed.</returns>
    protected abstract Task<bool> RunCycle(TTask task);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        InitServices();

        logger.LogInformation("{WorkerName} is running", WorkerName);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (!TryGetNextTask(out var task))
            {
                logger.LogDebug("No task to process, waiting {NoTaskTimeout}", NoTaskTimeout);
                await Task.Delay(NoTaskTimeout, stoppingToken);
                continue;
            }
            
            try
            {
                await CalculatorTaskService.SetTaskRunning<TTask>(task!.Id);
                var anyWorkDone = await RunCycle(task);

                var resultTaskStatus =
                    anyWorkDone ? CalculatorTaskStatus.Success : CalculatorTaskStatus.NothingToProcess;

                await CalculatorTaskService.SetTaskResult<TTask>(task.Id, resultTaskStatus);
            }
            catch (Exception e)
            {
                if (task != null)
                {
                    await CalculatorTaskService.SetTaskResult<TTask>(task.Id, CalculatorTaskStatus.Error);
                }
                
                logger.LogError(e, "{WorkerName} threw exception; {Message}", WorkerName, e.Message);
                await Task.Delay(10000, stoppingToken);
            }
        }
    }

    protected bool TryGetNextTask(out TTask? task)
    {
        task = CalculatorTaskService.GetNextTask<TTask>().Result;
        return task != null;
    }

    public override void Dispose()
    {
        base.Dispose();
        ServiceScope.Dispose();
    }
}