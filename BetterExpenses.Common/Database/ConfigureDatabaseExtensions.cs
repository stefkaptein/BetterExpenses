using BetterExpenses.Common.Database.Mongo;
using BetterExpenses.Common.Database.Sql;
using BetterExpenses.Common.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BetterExpenses.Common.Database;

public static class ConfigureDatabaseExtensions
{
    public static IServiceCollection ConfigurePostgres(this IServiceCollection services, ConfigurationManager configuration, Action<DbContextOptionsBuilder>? additionalOptions = null)
    {
        services.AddDbContext<SqlDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Postgres"));
            additionalOptions?.Invoke(options);
        });
        
        return services;
    }

    public static IServiceCollection ConfigureMongo(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.Configure<MongoOptions>(configuration.GetSection("Mongo"));

        services.AddSingleton<IMongoConnection, MongoConnection>();
        
        return services;
    }
}