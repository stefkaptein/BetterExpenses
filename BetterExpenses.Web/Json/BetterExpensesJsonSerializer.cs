using System.Text.Json;
using BetterExpenses.Web.Json.Exceptions;

namespace BetterExpenses.Web.Json;

public static class BetterExpensesJsonSerializer
{
    private static readonly JsonSerializerOptions? JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public static T? Deserialize<T>(string jsonString) =>
        JsonSerializer.Deserialize<T>(jsonString, JsonSerializerOptions);

    public static T DeserializeRequired<T>(string jsonString) =>
        Deserialize<T>(jsonString) ?? throw new SerializationException(typeof(T), jsonString);
}