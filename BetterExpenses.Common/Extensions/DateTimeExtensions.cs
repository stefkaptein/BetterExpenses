namespace BetterExpenses.Common.Extensions;

public static class DateTimeExtensions
{
    public static DateOnly ToDateOnly(this DateTime dt) => DateOnly.FromDateTime(dt);

    /// <summary>
    /// Return the number of months the other date is before the current date.
    ///
    /// This method throws an exception if other date is after this date.
    /// </summary>
    /// <param name="thisDate">The date to compare to</param>
    /// <param name="otherDate">The date to calculate the difference to</param>
    /// <returns>Number of months other date is before this date</returns>
    public static int GetNumberOfMonthsBack(this DateTime thisDate, DateTime otherDate)
    {
        if (otherDate > thisDate)
        {
            throw new ArgumentException("Other date must be before this date");
        }
        return (thisDate.Year - otherDate.Year) * 12 + thisDate.Month - otherDate.Month;
    }
}