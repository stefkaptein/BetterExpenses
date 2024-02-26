using BetterExpenses.API.Services.Auth;
using BetterExpenses.API.Services.Options;

namespace BetterExpenses.API.Services;

public static class ConfigureServicesExtensions
{
    public static IServiceCollection BindConfiguration(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<OAuthClientOptions>(configuration);
        services.Configure<AuthOptions>(configuration.GetSection("Auth"));
        
        return services;
    }
    
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<IBunqAuthService, BunqAuthService>();
        
        return services;
    }
}