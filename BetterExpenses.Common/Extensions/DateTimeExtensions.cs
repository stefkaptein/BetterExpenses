namespace BetterExpenses.Common.Extensions;

public static class DateTimeExtensions
{
    public static DateOnly ToDateOnly(this DateTime dt) => DateOnly.FromDateTime(dt);
}