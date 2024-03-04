namespace BetterExpenses.Web.Json.Exceptions;

public class SerializationException(Type type, string jsonString) : Exception
{
    public Type Type { get; } = type;
    public string JsonString { get; } = jsonString;

    public override string ToString()
    {
        return $"Failed to deserialize JSON string to type {Type.Name}:\n{JsonString}";
    }
}