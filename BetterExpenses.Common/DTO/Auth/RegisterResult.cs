namespace BetterExpenses.Common.DTO.Auth;

public class RegisterResult
{
    public bool Successful { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}