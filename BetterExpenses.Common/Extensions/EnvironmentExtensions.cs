namespace BetterExpenses.Common.Extensions;

public static class EnvironmentExtensions
{
    public static bool EnvironmentIsDevelopment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
    }   
}