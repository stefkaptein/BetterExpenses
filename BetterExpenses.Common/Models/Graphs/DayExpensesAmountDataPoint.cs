using MongoDB.Bson.Serialization.Attributes;

namespace BetterExpenses.Common.Models.Graphs;

public class DayDataPointKey
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
}

public class DayExpensesAmountDataPoint
{
    [BsonElement("_id")] public DayDataPointKey Id { get; set; } = null!;

    public double Amount { get; set; }
}