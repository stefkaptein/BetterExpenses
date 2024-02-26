using System.ComponentModel.DataAnnotations;

namespace BetterExpenses.Common.Models.User;

public class UserOptions
{
    public static readonly TimeSpan DefaultFetchPaymentsFrom = TimeSpan.FromDays(90);
    
    [Key]
    public Guid BetterExpensesUserId { get; set; }
    public TimeSpan FetchPaymentsFrom { get; set; } = DefaultFetchPaymentsFrom;
}