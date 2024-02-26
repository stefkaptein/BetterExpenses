namespace BetterExpenses.API.Services.Options;

public class AuthOptions
{
    public string RedirectUri { get; set; }
    public string BaseUri { get; set; }
    public string AuthRoute { get; set; }
    public string TokenRoute { get; set; }
}