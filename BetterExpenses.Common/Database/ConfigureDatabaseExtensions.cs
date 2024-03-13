using BetterExpenses.Common.Database.Mongo;
using BetterExpenses.Common.Database.Sql;
using BetterExpenses.Common.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace BetterExpenses.Common.Database;

public static class ConfigureDatabaseExtensions
{
    public static IServiceCollection ConfigurePostgres(this IServiceCollection services,
        ConfigurationManager configuration, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped,
        Action<DbContextOptionsBuilder>? additionalOptions = null)
    {
        services.AddDbContext<SqlDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Postgres"));
            additionalOptions?.Invoke(options);
        }, serviceLifetime);

        return services;
    }

    public static IServiceCollection ConfigureMongo(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.Configure<MongoOptions>(configuration.GetSection("Mongo"));
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
#pragma warning disable CS0618 // Type or member is obsolete
        BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
        BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
#pragma warning restore CS0618 // Type or member is obsolete

        services.AddSingleton<IMongoConnection, MongoConnection>();

        return services;
    }
}