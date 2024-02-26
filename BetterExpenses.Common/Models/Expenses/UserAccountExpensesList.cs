using MongoDB.Bson.Serialization.Attributes;

namespace BetterExpenses.Common.Models.Expenses;

public class UserAccountExpensesList
{
    [BsonId, BsonElement("_id")] public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }

    public int AccountId { get; set; }
    
    public List<UserExpense> Expenses { get; set; } = [];
}