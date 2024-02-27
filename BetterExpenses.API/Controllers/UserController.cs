using BetterExpenses.Common.Services.MonetaryAccounts;
using BetterExpenses.Common.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterExpenses.API.Controllers;

public class UserController(IMonetaryAccountService monetaryAccountService, IUserOptionsService userOptionsService)
    : AuthorizedApiControllerBase
{
    private readonly IMonetaryAccountService _monetaryAccountService = monetaryAccountService;
    private readonly IUserOptionsService _userOptionsService = userOptionsService;
}