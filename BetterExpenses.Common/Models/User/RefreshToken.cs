using System.ComponentModel.DataAnnotations;

namespace BetterExpenses.Common.Models.User;

public class RefreshToken
{
    [Key]
    public required string Token { get; set; }

    public required DateTime Expires { get; set; }

    public BetterExpensesUser User { get; set; } = null!;

    public required bool Valid { get; set; }
    
    public required Guid UserId { get; set; }
}