using Bunq.Sdk.Model.Generated.Endpoint;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BetterExpenses.Common.Models.Graphs;

public class ExpensesGraph
{
    [BsonId, BsonElement("_id")] public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }

    public List<DayExpensesAmountDataPoint> DataPoints { get; set; }
}