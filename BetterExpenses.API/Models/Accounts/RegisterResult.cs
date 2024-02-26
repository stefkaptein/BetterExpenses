namespace BetterExpenses.API.Models.Accounts;

public class RegisterResult
{
    public bool Successful { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}