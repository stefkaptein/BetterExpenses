namespace BetterExpenses.Common.Models.User;

public class UserSettings
{
    public static readonly TimeSpan DefaultFetchPaymentsFrom = TimeSpan.FromDays(90);
    
    public Guid Id { get; set; }
    
    public BetterExpensesUser User { get; set; }
    
    public TimeSpan FetchPaymentsFrom { get; set; } = DefaultFetchPaymentsFrom;
    
    public bool BunqLinked { get; set; }
}