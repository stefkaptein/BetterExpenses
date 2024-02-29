using BetterExpenses.API.Services.Auth;
using BetterExpenses.API.Services.Options;
using BetterExpenses.Common.Database.Sql;
using BetterExpenses.Common.Models.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BetterExpenses.API.Services;

public static class ConfigureAuthenticationExtensions
{
    public static IServiceCollection ConfigureOpenIdServer(this IServiceCollection services)
    {
        services.AddOpenIddict()
            .AddCore(
                options =>
                {
                    //register stores 
                    options.UseEntityFrameworkCore()
                        .UseDbContext<SqlDbContext>();
                }
            )
            .AddServer(
                options =>
                {
                    // Enable the token endpoint.
                    options.SetTokenEndpointUris("connect/token");

                    // Enable the client credentials flow.
                    options.AllowClientCredentialsFlow();

                    // Register the signing and encryption credentials.
                    options.AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();

                    // Register the ASP.NET Core host and configure the ASP.NET Core options.
                    options.UseAspNetCore()
                        .EnableTokenEndpointPassthrough();
                }
            );
        return services;
    }
    
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        var jwtSecret = configuration["JwtSecret"];
        if (jwtSecret == null)
        {
            throw new Exception("JwtSecret not configured");
        }

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.Configure<JwtOptions>(options =>
        {
            options.Issuer = configuration["Jwt:Issuer"]!;
            options.Audience = configuration["Jwt.Issuer"]!;
            options.Secret = jwtSecret;
        });

        services.AddIdentityCore<BetterExpensesUser>()
            .AddEntityFrameworkStores<SqlDbContext>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    
                    options.Audience = configuration["Jwt:Audience"];
                    options.Authority = configuration["Jwt:Issuer"];
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = false,
                        ClockSkew = TimeSpan.FromSeconds(100),
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtSecret))
                    };
                });

        services.AddAuthorization();
        return services;
    }
}