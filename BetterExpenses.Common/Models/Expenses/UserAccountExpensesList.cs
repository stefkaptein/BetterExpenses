using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BetterExpenses.Common.Models.Expenses;

public class UserAccountExpensesList
{
    [BsonId, BsonElement("_id")] public Guid Id { get; set; } = Guid.NewGuid();
    
    public required Guid UserId { get; set; }

    public required int AccountId { get; set; }

    public required DateTime FetchedUntil { get; set; }
    
    public required List<UserExpense> Expenses { get; set; } = [];
}