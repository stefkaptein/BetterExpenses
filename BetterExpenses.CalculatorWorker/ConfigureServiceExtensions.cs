using BetterExpenses.CalculatorWorker.Workers;
using BetterExpenses.CalculatorWorker.Workers.Accounts;
using BetterExpenses.CalculatorWorker.Workers.Expenses.Fetching;
using BetterExpenses.CalculatorWorker.Workers.Expenses.Processing;
using BetterExpenses.CalculatorWorker.Workers.Expenses.Processing.Graphs;
using BetterExpenses.Common.Database;
using BetterExpenses.Common.Services;

namespace BetterExpenses.CalculatorWorker;

public static class ConfigureServiceExtensions
{
    private static IServiceCollection AddWorkerServices(this IServiceCollection services)
    {
        services.AddScoped<IFetchAccountsTaskRunner, FetchAccountsTaskRunner>();
        services.AddScoped<IFetchExpensesTaskRunner, FetchExpensesTaskRunner>();
        services.AddScoped<IProcessExpensesTaskRunner, ProcessExpensesTaskRunner>();
        return services;
    }
    
    private static IServiceCollection AddHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<FetchAccountsWorker>();
        services.AddHostedService<FetchExpensesWorker>();
        services.AddHostedService<ProcessExpensesWorker>();
        return services;
    }

    private static IServiceCollection AddGraphCreators(this IServiceCollection services)
    {
        services.AddScoped<MonthlyExpensesGraphCreator>();
        return services;
    }
    
    public static IServiceCollection ConfigureServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.BindCommonConfiguration(configuration);

        services.ConfigureCommonServices();
        services.AddWorkerServices();
        services.AddGraphCreators();
        services.AddHostedServices();

        services.ConfigurePostgres(configuration);
        services.ConfigureMongo(configuration);
        
        return services;
    }
}