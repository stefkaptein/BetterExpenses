using BetterExpenses.Common.Models.Tasks;
using BetterExpenses.Common.Services.Tasks;
using BetterExpenses.Common.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace BetterExpenses.API.Controllers;

public class UserTaskController(ICalculatorTaskService calculatorTaskService, IUserOptionsService userOptionsService) : AuthorizedApiControllerBase
{
    private readonly ICalculatorTaskService _calculatorTaskService = calculatorTaskService;
    private readonly IUserOptionsService _userOptionsService = userOptionsService;

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
        var fetchTill = await _userOptionsService.GetFetchPaymentsTillForUser(user.Id);

        var task = new FetchExpensesTask
        {
            UserId = user.Id,
            FetchTill = fetchTill
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