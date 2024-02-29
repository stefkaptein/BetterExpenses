namespace BetterExpenses.API.Exceptions;

public class ApiAuthenticationException(string url) : Exception
{
    public override string Message => $"User not authenticated while controller expected a authenticated user\n{url}";
}