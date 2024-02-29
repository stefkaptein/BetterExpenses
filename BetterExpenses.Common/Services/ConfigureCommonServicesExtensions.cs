using BetterExpenses.Common.Options;
using BetterExpenses.Common.ServiceModels;
using BetterExpenses.Common.Services.Bunq;
using BetterExpenses.Common.Services.Context;
using BetterExpenses.Common.Services.Crypto;
using BetterExpenses.Common.Services.Expenses;
using BetterExpenses.Common.Services.MonetaryAccounts;
using BetterExpenses.Common.Services.Tasks;
using BetterExpenses.Common.Services.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BetterExpenses.Common.Services;

public static class ConfigureCommonServicesExtensions
{
    public static IServiceCollection BindCommonConfiguration(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<CryptoOptions>(configuration);
        services.Configure<BunqOptions>(configuration.GetSection("Bunq"));
        services.Configure<ApiContextOptions>(configuration.GetSection("ApiContext"));
        services.Configure<ApiContextFileOptions>(configuration.GetSection("ApiContextFile"));
        
        return services;
    }
    
    public static IServiceCollection ConfigureCommonServices(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        services.AddSingleton<ICryptoService, CryptoService>();
        services.AddSingleton<ApiContextCache>();
        
        services.AddScoped<IApiContextCryptoFileService, ApiContextCryptoFileService>();
        services.AddScoped<IApiContextService, ApiContextService>();

        services.AddScoped<ICalculatorTaskService, CalculatorTaskService>();
        services.AddScoped<IMonetaryAccountService, MonetaryAccountService>();

        services.AddScoped<IExpensesMongoService, ExpensesMongoService>();
        services.AddScoped<IUserOptionsService, UserOptionsService>();

        services.ConfigureBunqApiServices();
        
        return services;
    }
}