using BetterExpenses.Common.Services.Mongo;
using MongoDB.Bson.Serialization.Attributes;

namespace BetterExpenses.Common.Models.Expenses;

[MongoCollection(CollectionName = CollectionName)]
public class FixedUserExpense
{
    public const string CollectionName = "FixedUserExpenses";
    
    [BsonId, BsonElement("_id")] public Guid Id { get; set; } = Guid.NewGuid();

    public required Guid UserId { get; set; }
    
    public required int MonetaryAccountId { get; set; }

    public required double Amount { get; set; }

    public required int ExpectedDay { get; set; }

    public required string CounterPartyIban { get; set; }
    
    public required string CounterPartyDescription { get; set; }
    
    public required List<int> PreviousPaymentIds { get; set; }
}