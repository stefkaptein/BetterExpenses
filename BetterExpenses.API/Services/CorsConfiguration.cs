namespace BetterExpenses.API.Services;

public static class CorsConfiguration
{
    public const string BlazorWebAppPolicy = "Blazor Web App";
    private const string LocalApiUrl = "https://localhost:7086";
    
    public const string BunqOauthApi = "Bunq Oath API";
    private const string BunqOathApiUrl = "https://oauth.bunq.com";

    public static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        if (EnvironmentIsDevelopment())
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: BlazorWebAppPolicy,
                    policy  =>
                    {
                        policy.WithOrigins(LocalApiUrl)
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
                options.AddPolicy(name: BunqOauthApi,
                    policy =>
                    {
                        policy.WithOrigins(BunqOathApiUrl)
                            .WithMethods("GET");
                    });
            });
        }

        return services;
    }
}