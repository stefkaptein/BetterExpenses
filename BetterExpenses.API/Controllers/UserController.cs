using BetterExpenses.Common.DTO.User;
using BetterExpenses.Common.Models.Tasks;
using BetterExpenses.Common.Services.MonetaryAccounts;
using BetterExpenses.Common.Services.Tasks;
using BetterExpenses.Common.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace BetterExpenses.API.Controllers;

public class UserController(
    IMonetaryAccountService monetaryAccountService,
    IUserOptionsService userOptionsService,
    ICalculatorTaskService calculatorTaskService)
    : AuthorizedApiControllerBase
{
    private readonly IMonetaryAccountService _monetaryAccountService = monetaryAccountService;
    private readonly IUserOptionsService _userOptionsService = userOptionsService;
    private readonly ICalculatorTaskService _calculatorTaskService = calculatorTaskService;

    [HttpGet]
    public async Task<IActionResult> Settings()
    {
        var user = await GetUser();

        var userSettings = await _userOptionsService.GetOptionsForUser(user.Id);
        if (userSettings == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<UserSettingsDto>(userSettings));
    }

    [HttpGet]
    public async Task<IActionResult> MonetaryAccounts()
    {
        var user = await GetUser();

        var accounts = await _monetaryAccountService.GetAccounts(user.Id);

        return Ok(accounts);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAnalyseAccount(int accountId, bool analyse)
    {
        var user = await GetUser();
        await _monetaryAccountService.SetAccountsToAnalyse(user.Id,
            new Dictionary<int, bool> { { accountId, analyse } });

        var task = new FetchExpensesTask
        {
            UserId = user.Id,
            FetchTill = DateTime.Today.Subtract(user.UserSettings.FetchPaymentsFrom).ToUniversalTime()
        };

        await _calculatorTaskService.AddTask(task);

        return Ok();
    }
}