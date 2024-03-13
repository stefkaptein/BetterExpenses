using BetterExpenses.Common.Options;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BetterExpenses.Common.Database.Mongo;

public interface IMongoConnection
{
    public IMongoCollection<T> GetCollection<T>(string name);
}

public class MongoConnection : IMongoConnection
{
    
    
    
    
    private readonly IMongoDatabase _database;
    
    public MongoConnection(IOptions<MongoOptions> mongoOptions)
    {
        var mongoOptionsValue = mongoOptions.Value;
        var connection = new MongoClient(mongoOptionsValue.ConnectionString);
        
        _database = connection.GetDatabase(mongoOptionsValue.Database);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }
}