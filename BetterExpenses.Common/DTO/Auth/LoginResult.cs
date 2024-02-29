namespace BetterExpenses.Common.DTO.Auth;

public record TokenWithExperation(string Token, DateTime Expires);

public class LoginResult
{
    public bool Successful { get; set; }
    public string? AuthToken { get; set; }
    public TokenWithExperation? RefreshToken { get; set; }
    public string? Error { get; set; }
}