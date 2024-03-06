using BetterExpenses.API.Services.Graph;
using Microsoft.AspNetCore.Mvc;

namespace BetterExpenses.API.Controllers;

public class ChartController(IGraphService graphService) : AuthorizedApiControllerBase
{
    private readonly IGraphService _graphService = graphService;

    [HttpGet]
    public async Task<IActionResult> TotalExpenses(int year, int month)
    {
        var user = await GetUser();

        var chart = await _graphService.GetTotalExpensesChart(user.Id, year, month);
        if (chart == null)
        {
            return NotFound();
        }
        
        return Ok(chart);
    }
}