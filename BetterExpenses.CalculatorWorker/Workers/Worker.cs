namespace BetterExpenses.CalculatorWorker.Workers;

public abstract class Worker(IServiceScopeFactory serviceScopeFactory) : BackgroundService, IDisposable
{
    protected readonly IServiceScope ServiceScope = serviceScopeFactory.CreateScope();

    public void Dispose()
    {
        ServiceScope.Dispose();
    }
}