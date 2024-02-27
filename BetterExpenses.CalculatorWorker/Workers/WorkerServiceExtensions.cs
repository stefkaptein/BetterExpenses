using BetterExpenses.CalculatorWorker.Workers.Accounts;
using BetterExpenses.CalculatorWorker.Workers.Expenses;
using BetterExpenses.CalculatorWorker.Workers.Expenses.Fetching;
using BetterExpenses.CalculatorWorker.Workers.Expenses.Processing;

namespace BetterExpenses.CalculatorWorker.Workers;

public static class WorkerServiceExtensions
{
    public static IServiceCollection AddWorkerServices(this IServiceCollection services)
    {
        services.AddScoped<IFetchAccountsTaskRunner, FetchAccountsTaskRunner>();
        services.AddScoped<IFetchExpensesTaskRunner, FetchExpensesTaskRunner>();
        services.AddScoped<IProcessExpensesTaskRunner, ProcessExpensesTaskRunner>();
        return services;
    }
}