using System.Runtime.InteropServices.JavaScript;
using BetterExpenses.CalculatorWorker.Models.Aggregations;
using BetterExpenses.Common.Models.Expenses;
using F23.StringSimilarity;

namespace BetterExpenses.CalculatorWorker.Workers.Expenses.Processing.Graphs.RecurringExpenses;

public class RecurringExpensesClassifier
{
    /// <summary>
    /// Minimum number of months a transaction needs to happen before it is considered recurring.
    /// </summary>
    private const int MinimumConsecutiveDays = 3;
    
    /// <summary>
    /// The maximum value by which the days may deviate from the average day, on average.
    /// </summary>
    private const double MaximumAverageDeviationFromAverageDay = 1.0;
    
    /// <summary>
    /// The maximum normalized distance where two descriptions are considered consistent.
    /// </summary>
    private const double MaximumDescriptionSimilarityDistance = 0.5;

    public FixedUserExpense GetFixedUserExpense(RecurringExpensesAggregation expensesAggregation, Guid userId)
    {
        var paymentsSorted = expensesAggregation.Payments
            .OrderByDescending(x => x.Updated)
            .ToList();

        return new FixedUserExpense
        {
            UserId = userId,
            MonetaryAccountId = GetMonetaryAccountId(paymentsSorted),
            Amount = GetAmount(paymentsSorted),
            ExpectedDay = GetExpectedDay(expensesAggregation.Dates),
            CounterPartyIban = expensesAggregation.Id.Iban,
            CounterPartyDescription = expensesAggregation.Id.DisplayName,
            PreviousPaymentIds = paymentsSorted.Select(x => x.Id).ToList()
        };
    }

    private static int GetMonetaryAccountId(IEnumerable<UserExpense> paymentsSorted)
    {
        return paymentsSorted.First().MonetaryAccountId;
    }
    
    private static double GetAmount(IEnumerable<UserExpense> paymentsSorted)
    {
        return paymentsSorted.First().Amount;
    }

    private static int GetExpectedDay(IEnumerable<DateInElements> paymentDates)
    {
        var averageDay = paymentDates.Select(x => x.Day).Average();
        return (int)double.Ceiling(averageDay);
    }

    public bool IsFixedUserExpense(RecurringExpensesAggregation expensesAggregation)
    {
        if (!HasRecurrentDates(expensesAggregation.Dates))
        {
            return false;
        }

        if (!HasConsistentDescriptions(expensesAggregation.Payments))
        {
            return false;
        }

        if (!AmountsAreConsistent(expensesAggregation))
        {
            return false;
        }

        return true;
    }
    
    private bool AmountsAreConsistent(RecurringExpensesAggregation expensesAggregation)
    {
        return true;
    }

    private static bool HasConsistentDescriptions(IReadOnlyCollection<UserExpense> expenses)
    {
        var descriptions = expenses.Select(x => x.Description).ToList();
        if (descriptions.Any(string.IsNullOrEmpty))
        {
            return false;
        }

        var n = descriptions.Count;
        
        for (var i = 0; i < n; i++)
        {
            for (var j = i + 1; j < n; j++)
            {
                var metric = new NormalizedLevenshtein();
                var distance = metric.Distance(descriptions[i], descriptions[j]);
                if (distance > MaximumDescriptionSimilarityDistance)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static bool HasRecurrentDates(IReadOnlyCollection<DateInElements> dates)
    {
        var sortedDates = dates
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .ToList();
        
        var averageDay = dates.Select(x => x.Day).Average();
        
        var firstDate = sortedDates.First();

        var curYear = firstDate.Year;
        var curMonth = firstDate.Month;
        var cumDifFromDayMean = Math.Abs(firstDate.Day - averageDay);
        var consecutiveDays = 1;

        foreach (var date in sortedDates.Skip(1))
        {
            if (date.Month == curMonth - 1 && date.Year == curYear)
            {
                curMonth = date.Month;
            }
            else if (date.Month == curMonth + 11 && date.Year == curYear - 1)
            {
                curMonth = date.Month;
                curYear = date.Year;
            }
            else
            {
                break;
            }

            cumDifFromDayMean += Math.Abs(firstDate.Day - averageDay);
            consecutiveDays++;
        }

        return consecutiveDays >= MinimumConsecutiveDays &&
               !(cumDifFromDayMean / consecutiveDays > MaximumAverageDeviationFromAverageDay);
    }
}