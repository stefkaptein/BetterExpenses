namespace BetterExpenses.CalculatorWorker.Workers.Expenses.Processing;

public interface IProcessExpensesTaskRunner
{
    public Task<bool> RunCycle();
}

public class ProcessExpensesTaskRunner : IProcessExpensesTaskRunner
{
    public Task<bool> RunCycle()
    {
        throw new NotImplementedException();
    }
}