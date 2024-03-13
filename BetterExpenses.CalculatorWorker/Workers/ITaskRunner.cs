namespace BetterExpenses.CalculatorWorker.Workers;

public interface ITaskRunner<TTask>
{
    public Task<bool> RunCycle(TTask task);
}