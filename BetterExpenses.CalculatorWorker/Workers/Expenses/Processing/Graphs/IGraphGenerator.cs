namespace BetterExpenses.CalculatorWorker.Workers.Expenses.Processing.Graphs;

public interface IGraphGenerator
{
    public Task Execute(Guid userId);
}