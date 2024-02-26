using BetterExpenses.Common.Models.User;

namespace BetterExpenses.Common.Models.Tasks;

public class FetchExpensesTask : CalculatorTask
{
    public Guid UserId { get; set; }
    public DateTime FetchTill { get; set; } = DateTime.Today.Subtract(UserOptions.DefaultFetchPaymentsFrom).ToUniversalTime();
}