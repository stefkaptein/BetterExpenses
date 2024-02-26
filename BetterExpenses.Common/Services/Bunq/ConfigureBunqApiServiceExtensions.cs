using Microsoft.Extensions.DependencyInjection;

namespace BetterExpenses.Common.Services.Bunq;

public static class ConfigureBunqApiServiceExtensions
{
    public static IServiceCollection ConfigureBunqApiServices(this IServiceCollection services)
    {
        services.AddScoped<IBunqExpensesService, BunqExpensesService>();
        services.AddScoped<IBunqMonetaryAccountService, BunqMonetaryAccountService>();
        return services;
    }
}