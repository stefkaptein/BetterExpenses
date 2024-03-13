using BetterExpenses.Common.Database.Mongo;
using BetterExpenses.Common.Models.Graphs;
using BetterExpenses.Common.Services.Mongo.Expenses;
using MongoDB.Driver;

namespace BetterExpenses.API.Services.Graph;

public interface IGraphService
{
    public Task<LineChart?> GetTotalExpensesChart(Guid userId, int year, int month);
}

public class GraphService(IExpensesGraphMongoService expensesGraphMongoService) : IGraphService
{
    private readonly IExpensesGraphMongoService _expensesGraphMongoService = expensesGraphMongoService;

    public async Task<LineChart?> GetTotalExpensesChart(Guid userId, int year, int month)
    {
        var graph = await _expensesGraphMongoService.GetGraphForUser(userId);
        if (graph == null)
        {
            return null;
        }

        var dataPoints = graph.DataPoints
            .Where(x => x.Id.Year == year && x.Id.Month == month)
            .ToList();

        var numberOfDays = dataPoints.Select(x => x.Id.Day).Max();

        var labels = Enumerable.Range(1, numberOfDays).Select(x => x.ToString()).ToList();
        
        var dayExpenseData = dataPoints.Select(x => -x.Amount).ToList();

        var sum = 0.0;
        var cumulativeExpenseData = dataPoints.Select(x => sum += -x.Amount).ToList();

        return new LineChart
        {
            Labels = labels,
            DataDictionary = new Dictionary<string, List<double>>
            {
                { "Day", dayExpenseData },
                { "Cumulative", cumulativeExpenseData }
            }
        };
    }
}