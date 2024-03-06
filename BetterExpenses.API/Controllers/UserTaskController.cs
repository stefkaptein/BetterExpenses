using BetterExpenses.Common.Models.Tasks;
using BetterExpenses.Common.Services.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BetterExpenses.API.Controllers;

public class UserTaskController(ICalculatorTaskService calculatorTaskService) : AuthorizedApiControllerBase
{
    private readonly ICalculatorTaskService _calculatorTaskService = calculatorTaskService;

    [HttpGet]
    public async Task<IActionResult> FetchMonetaryAccounts()
    {
        var user = await GetUser();
        
        var task = new FetchAccountsTask
        {
            UserId = user.Id,
            Overwrite = true
        };
        
        await _calculatorTaskService.AddTask(task);
        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> FetchExpenses()
    {
        var user = await GetUser();

        var task = new FetchExpensesTask
        {
            UserId = user.Id,
            FetchTill = DateTime.Today.Subtract(user.UserSettings.FetchPaymentsFrom).ToUniversalTime()
        };
        
        await _calculatorTaskService.AddTask(task);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> ProcessExpenses()
    {
        var user = await GetUser();

        var task = new ProcessExpensesTask
        {
            UserId = user.Id
        };
        
        await _calculatorTaskService.AddTask(task);
        return Ok();
    }
}