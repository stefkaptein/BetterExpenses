using BetterExpenses.Common.Models.Tasks;
using BetterExpenses.Common.Models.User;
using BetterExpenses.Common.Services.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BetterExpenses.API.Controllers;

[ApiController, Authorize]
[Route("[controller]/[action]")]
public class UserTaskController(
    ICalculatorTaskService calculatorTaskService,
    UserManager<BetterExpensesUser> userManager) : ControllerBase
{
    private readonly ICalculatorTaskService _calculatorTaskService = calculatorTaskService;

    [HttpGet]
    public async Task<IActionResult> FetchMonetaryAccounts()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        
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
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var task = new FetchExpensesTask
        {
            UserId = user.Id,
            FetchTill = DateTime.Today.Subtract(user.UserOptions.FetchPaymentsFrom)
        };
        
        await _calculatorTaskService.AddTask(task);
        return Ok();
    }
}