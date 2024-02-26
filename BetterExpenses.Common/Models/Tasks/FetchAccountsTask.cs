namespace BetterExpenses.Common.Models.Tasks;

public class FetchAccountsTask : CalculatorTask
{
    public Guid UserId { get; set; }
    public bool Overwrite { get; set; }
}