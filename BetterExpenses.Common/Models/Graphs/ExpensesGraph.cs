using BetterExpenses.Common.Services.Mongo;
using Bunq.Sdk.Model.Generated.Endpoint;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BetterExpenses.Common.Models.Graphs;

[MongoCollection(CollectionName = ExpensesGraphsCollectionName)]
public class ExpensesGraph
{
    public const string ExpensesGraphsCollectionName = "UserTotalExpensesGraphs";
    
    [BsonId, BsonElement("_id")] public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }

    public List<DayExpensesAmountDataPoint> DataPoints { get; set; }
}