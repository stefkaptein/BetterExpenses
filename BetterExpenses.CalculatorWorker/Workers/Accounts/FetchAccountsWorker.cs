using BetterExpenses.Common.Models.Tasks;

namespace BetterExpenses.CalculatorWorker.Workers.Accounts;

public class FetchAccountsWorker(ILogger<FetchAccountsWorker> logger, IServiceScopeFactory serviceScopeFactory)
    : Worker<FetchAccountsWorker, FetchAccountsTask>(serviceScopeFactory, logger)
{
    private IFetchAccountsTaskRunner _fetchAccountsTaskRunner = null!;
    
    protected override string WorkerName => "Fetch accounts worker";
    
    protected override void InitServices()
    {
        base.InitServices();
        _fetchAccountsTaskRunner = ServiceScope.ServiceProvider.GetRequiredService<IFetchAccountsTaskRunner>();
    }

    protected override async Task<bool> RunCycle(FetchAccountsTask task)
    {
        return await _fetchAccountsTaskRunner.RunCycle(task);
    }
}