namespace BetterExpenses.Common.Models.Exceptions;

public class NoApiContextForUserException(string userId) : Exception
{
    public string UserId { get; set; } = userId;
}