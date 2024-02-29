using BetterExpenses.Common.DTO.User;
using BetterExpenses.Common.Services.MonetaryAccounts;
using BetterExpenses.Common.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace BetterExpenses.API.Controllers;

public class UserController(IMonetaryAccountService monetaryAccountService, IUserOptionsService userOptionsService)
    : AuthorizedApiControllerBase
{
    private readonly IMonetaryAccountService _monetaryAccountService = monetaryAccountService;
    private readonly IUserOptionsService _userOptionsService = userOptionsService;

    [HttpGet]
    public async Task<IActionResult> GetUserSettings()
    {
        var user = await GetUser();

        var userSettings = await _userOptionsService.GetOptionsForUser(user.Id);
        if (userSettings == null)
        {
            return NotFound();
        }

        return Ok(Mapper.Map<UserSettingsDto>(userSettings));
    }
}