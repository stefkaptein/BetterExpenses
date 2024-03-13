namespace BetterExpenses.Common.Services.Mongo;

[AttributeUsage(AttributeTargets.Class)]
public class MongoCollectionAttribute : Attribute
{
    public required string CollectionName { get; set; }
}