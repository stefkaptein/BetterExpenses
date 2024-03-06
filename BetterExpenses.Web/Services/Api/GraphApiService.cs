using BetterExpenses.Common.Models.Graphs;

namespace BetterExpenses.Web.Services.Api;

public interface IGraphApiService
{
    public Task<LineChart> GetTotalExpensesChart(int year, int month);
}

public class GraphApiService(HttpClient httpClient) : ApiService(httpClient), IGraphApiService
{
    private const string GetTotalExpensesChartPath = "api/chart/TotalExpenses";
    
    public async Task<LineChart> GetTotalExpensesChart(int year, int month)
    {
        return (await GetJson<LineChart>(GetTotalExpensesChartPath, new Dictionary<string, string>
        {
            { "year", year.ToString() },
            { "month", month.ToString() }
        })).ResponseModel;
        
    }
}