using Microsoft.AspNetCore.Identity;

namespace BetterExpenses.Common.Models.User;

public class BetterExpensesUser : IdentityUser<Guid>
{
    public List<UserMonetaryAccount> Accounts { get; set; } = [];
    public UserOptions UserOptions { get; set; } = new();
}