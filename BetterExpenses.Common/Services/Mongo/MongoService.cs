using BetterExpenses.Common.Database.Mongo;
using MongoDB.Driver;

namespace BetterExpenses.Common.Services.Mongo;

public interface IMongoService<TDocument>
{
    public Task InsertOne(TDocument toInsert);
    public Task InsertMany(List<TDocument> toInsert);
    public Task BulkWrite(List<WriteModel<TDocument>> writeModels);

    public Task<IAsyncCursor<TAggregationResult>> Aggregate<TAggregationResult>(
        PipelineDefinition<TDocument, TAggregationResult> pipelineDefinition);
}

public abstract class MongoService<TDocument>(IMongoConnection mongoConnection) : IMongoService<TDocument>
{
    protected IMongoConnection MongoConnection = mongoConnection;

    protected IMongoCollection<TDocument> Collection = mongoConnection.GetCollection<TDocument>(GetCollectionName());

    public async Task InsertOne(TDocument toInsert)
    {
        await Collection.InsertOneAsync(toInsert);
    }

    public async Task InsertMany(List<TDocument> toInsert)
    {
        if (toInsert.Count > 0)
        {
            await Collection.InsertManyAsync(toInsert);
        }
    }

    public async Task BulkWrite(List<WriteModel<TDocument>> writeModels)
    {
        if (writeModels.Count > 0)
        {
            await Collection.BulkWriteAsync(writeModels);
        }
    }

    public async Task<IAsyncCursor<TAggregationResult>> Aggregate<TAggregationResult>(
        PipelineDefinition<TDocument, TAggregationResult> pipelineDefinition)
    {
        return await Collection.AggregateAsync(pipelineDefinition);
    }

    private static string GetCollectionName()
    {
        var type = typeof(TDocument);
        var mongoCollectionAttribute = Attribute.GetCustomAttributes(type)
            .FirstOrDefault(x => x is MongoCollectionAttribute);
        if (mongoCollectionAttribute == null)
        {
            throw new ArgumentException(
                $"No MongoCollection is defined for type {type.Name}, can't initialize MongoService");
        }

        return ((MongoCollectionAttribute)mongoCollectionAttribute).CollectionName;
    }
}