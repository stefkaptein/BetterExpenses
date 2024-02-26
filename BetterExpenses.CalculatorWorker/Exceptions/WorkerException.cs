namespace BetterExpenses.CalculatorWorker.Exceptions;

public class WorkerException : Exception
{
    public WorkerException(string? message) : base(message)
    {
    }
}