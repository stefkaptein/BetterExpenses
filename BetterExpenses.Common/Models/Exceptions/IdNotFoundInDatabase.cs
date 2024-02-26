namespace BetterExpenses.Common.Models.Exceptions;

public class IdNotFoundInDatabase : Exception
{
    public string Id { get; set; }
    
    public IdNotFoundInDatabase(string id)
    {
        Id = id;
    }

    public IdNotFoundInDatabase(string id, string? message) : base(message)
    {
        Id = id;
    }

    public IdNotFoundInDatabase( string id, string? message, Exception? innerException) : base(message, innerException)
    {
        Id = id;
    }
}