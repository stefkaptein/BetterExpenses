using BetterExpenses.Common.Services.MonetaryAccounts;
using BetterExpenses.Common.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterExpenses.API.Controllers;

[ApiController, Authorize]
[Route("[controller]/[action]")]
public class UserController(IMonetaryAccountService monetaryAccountService, IUserOptionsService userOptionsService)
    : ControllerBase
{
    private readonly IMonetaryAccountService _monetaryAccountService = monetaryAccountService;
    private readonly IUserOptionsService _userOptionsService = userOptionsService;
}