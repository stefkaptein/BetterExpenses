namespace BetterExpenses.CalculatorWorker.Workers.Accounts;

public class FetchAccountsWorker(ILogger<FetchAccountsWorker> logger, IServiceScopeFactory serviceScopeFactory)
    : Worker<FetchAccountsWorker>(serviceScopeFactory, logger)
{
    private IFetchAccountsTaskRunner _fetchAccountsTaskRunner = null!;
    
    protected override string WorkerName => "Fetch accounts worker";
    
    protected override void InitServices()
    {
        _fetchAccountsTaskRunner = ServiceScope.ServiceProvider.GetRequiredService<IFetchAccountsTaskRunner>();
    }

    protected override async Task<bool> RunCycle()
    {
        return await _fetchAccountsTaskRunner.RunCycle();
    }
}