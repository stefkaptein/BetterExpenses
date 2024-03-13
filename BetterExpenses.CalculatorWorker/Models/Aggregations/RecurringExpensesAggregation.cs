using BetterExpenses.Common.Models.Expenses;
using MongoDB.Bson.Serialization.Attributes;

namespace BetterExpenses.CalculatorWorker.Models.Aggregations;

public record struct DateInElements(int Year, int Month, int Day);

public record struct IbanWithNameKey(string DisplayName, string Iban);

public class RecurringExpensesAggregation
{
    [BsonElement("_id")]
    public IbanWithNameKey Id { get; set; }

    public List<UserExpense> Payments { get; set; }
    
    public List<DateInElements> Dates { get; set; }

    public int NumberOfPayments { get; set; }

    public double Mean { get; set; }

    public double DifferenceWithMean { get; set; }
}