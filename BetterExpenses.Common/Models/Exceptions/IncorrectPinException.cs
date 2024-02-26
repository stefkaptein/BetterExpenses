namespace BetterExpenses.Common.Models.Exceptions;

public class IncorrectPinException : Exception
{
    public IncorrectPinException()
    {
    }

    public IncorrectPinException(string? message) : base(message)
    {
    }

    public IncorrectPinException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}