using BetterExpenses.Common.Database.Mongo;
using BetterExpenses.Common.Models.Graphs;
using MongoDB.Driver;

namespace BetterExpenses.API.Services.Graph;

public interface IGraphService
{
    public Task<LineChart?> GetTotalExpensesChart(Guid userId, int year, int month);
}

public class GraphService(IMongoConnection mongoConnection) : IGraphService
{
    private readonly IMongoConnection _mongoConnection = mongoConnection;
    
    private readonly IMongoCollection<ExpensesGraph> _expenseGraphCollection = 
        mongoConnection.GetCollection<ExpensesGraph>(MongoConnection.ExpensesGraphsCollectionName);

    public async Task<LineChart?> GetTotalExpensesChart(Guid userId, int year, int month)
    {
        var graphCursor = await _expenseGraphCollection.FindAsync(Builders<ExpensesGraph>.Filter.Eq(x => x.UserId, userId));
        if (!await graphCursor.MoveNextAsync())
        {
            return null;
        }

        var graph = graphCursor.Current.First();
        
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