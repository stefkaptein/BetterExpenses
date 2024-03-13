using Microsoft.AspNetCore.Identity;

namespace BetterExpenses.Common.Models.User;

public class BetterExpensesUser : IdentityUser<Guid>
{
    public List<RefreshToken> RefreshTokens { get; set; }
    
    public List<UserMonetaryAccount> Accounts { get; set; } = [];
    
    public UserSettings UserSettings { get; set; }
}