using BetterExpenses.CalculatorWorker.Workers;
using BetterExpenses.CalculatorWorker.Workers.Accounts;
using BetterExpenses.CalculatorWorker.Workers.Expenses.Fetching;
using BetterExpenses.CalculatorWorker.Workers.Expenses.Processing;
using BetterExpenses.CalculatorWorker.Workers.Expenses.Processing.Graphs;
using BetterExpenses.Common.Database;
using BetterExpenses.Common.Extensions;
using BetterExpenses.Common.Services;

namespace BetterExpenses.CalculatorWorker;

public static class ConfigureServiceExtensions
{
    private static IServiceCollection AddWorkerServices(this IServiceCollection services)
    {
        services.AddTransient<IFetchAccountsTaskRunner, FetchAccountsTaskRunner>();
        services.AddTransient<IFetchExpensesTaskRunner, FetchExpensesTaskRunner>();
        services.AddTransient<IProcessExpensesTaskRunner, ProcessExpensesTaskRunner>();
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
        var graphGenerators = AppDomain.CurrentDomain.GetAllImplementingTypesInDomain<IGraphGenerator>();

        foreach (var graphGenerator in graphGenerators)
        {
            services.AddTransient(graphGenerator);
        }
        return services;
    }
    
    public static IServiceCollection ConfigureServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.BindCommonConfiguration(configuration);

        services.ConfigureCommonServices();
        services.AddWorkerServices();
        services.AddGraphCreators();
        services.AddHostedServices();

        services.ConfigurePostgres(configuration, ServiceLifetime.Transient);
        services.ConfigureMongo(configuration);
        
        return services;
    }
    
}