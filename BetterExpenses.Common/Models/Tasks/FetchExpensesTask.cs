using BetterExpenses.Common.Models.User;

namespace BetterExpenses.Common.Models.Tasks;

public class FetchExpensesTask : CalculatorTask
{
    public DateTime FetchTill { get; set; } = DateTime.UtcNow.Date.Subtract(UserSettings.DefaultFetchPaymentsFrom).ToUniversalTime();
}