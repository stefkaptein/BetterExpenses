using BetterExpenses.CalculatorWorker.Workers.Accounts;
using BetterExpenses.CalculatorWorker.Workers.Expenses;

namespace BetterExpenses.CalculatorWorker.Workers;

public static class WorkerServiceExtensions
{
    public static IServiceCollection AddWorkerServices(this IServiceCollection services)
    {
        services.AddScoped<IFetchAccountsTaskRunner, FetchAccountsTaskRunner>();
        services.AddScoped<IFetchExpensesTaskRunner, FetchExpensesTaskRunner>();
        return services;
    }
}