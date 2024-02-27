using BetterExpenses.API.Models.Accounts;
using BetterExpenses.API.Models.Login;
using BetterExpenses.API.Services.Auth;
using BetterExpenses.Common.Models.Tasks;
using BetterExpenses.Common.Models.User;
using BetterExpenses.Common.Services.Context;
using BetterExpenses.Common.Services.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BetterExpenses.API.Controllers;

public class AuthController(
    IBunqAuthService bunqAuthService,
    IApiContextService apiContextService,
    ICalculatorTaskService calculatorTaskService,
    UserManager<BetterExpensesUser> userManager,
    IJwtTokenService tokenService)
    : AuthorizedApiControllerBase
{
    private readonly IApiContextService _apiContextService = apiContextService;
    private readonly ICalculatorTaskService _calculatorTaskService = calculatorTaskService;
    private readonly UserManager<BetterExpensesUser> _userManager = userManager;
    private readonly IJwtTokenService _tokenService = tokenService;

    private readonly LoginResult _loginFailed = new()
        { Successful = false, Error = "Username and password combination is invalid." };
    
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var newUser = new BetterExpensesUser { UserName = model.Email, Email = model.Email };

        var result = await _userManager.CreateAsync(newUser, model.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(x => x.Description);

            return Ok(new RegisterResult { Successful = false, Errors = errors });
        }

        return Ok(new RegisterResult { Successful = true });
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginModel login)
    {
        var user = await _userManager.FindByEmailAsync(login.Email);
        if (user == null)
        {
            return Unauthorized(_loginFailed);
        }

        if (!await _userManager.CheckPasswordAsync(user, login.Password))
        {
            return Unauthorized(_loginFailed);
        }

        var token = _tokenService.GenerateToken(user);

        return Ok(new LoginResult { Successful = true, Token = token });
    }

    [HttpGet]
    public async Task<IActionResult> LinkBunq()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        return Redirect(bunqAuthService.GetAuthUri(user.Id));
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Callback(string code, string state)
    {
        if (!Guid.TryParse(state, out var stateId))
        {
            return Unauthorized("Invalid state");
        }

        if (!bunqAuthService.TryGetUserIdForState(stateId, out var userId))
        {
            return Unauthorized("State expired or is invalid");
        }

        var accessToken = await bunqAuthService.GetAccessToken(code);
        await _apiContextService.CreateAndSaveNewApiContext(userId, accessToken);

        await AddFetchAccountTask(userId);

        return Ok("Ok");
    }

    private async Task AddFetchAccountTask(Guid userId)
    {
        var task = new FetchAccountsTask
        {
            UserId = userId,
            Overwrite = true
        };
        await _calculatorTaskService.AddTask(task);
    }
}