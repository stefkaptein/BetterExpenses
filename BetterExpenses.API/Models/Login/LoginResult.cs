using BetterExpenses.API.Services.Auth;

namespace BetterExpenses.API.Models.Login;

public class LoginResult
{
    public bool Successful { get; set; }
    public TokenWithExperation? Token { get; set; }
    public string? Error { get; set; }
}