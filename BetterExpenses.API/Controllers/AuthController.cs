using BetterExpenses.API.Services.Auth;
using BetterExpenses.Common.DTO.Auth;
using BetterExpenses.Common.Models.Tasks;
using BetterExpenses.Common.Models.User;
using BetterExpenses.Common.Services.Context;
using BetterExpenses.Common.Services.Tasks;
using BetterExpenses.Common.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterExpenses.API.Controllers;

public class AuthController(
    IBunqAuthService bunqAuthService,
    IApiContextService apiContextService,
    ICalculatorTaskService calculatorTaskService,
    IJwtTokenService tokenService,
    IUserOptionsService userOptionsService)
    : AuthorizedApiControllerBase
{
    private readonly IBunqAuthService _bunqAuthService = bunqAuthService;
    private readonly IApiContextService _apiContextService = apiContextService;
    private readonly ICalculatorTaskService _calculatorTaskService = calculatorTaskService;
    private readonly IJwtTokenService _tokenService = tokenService;
    private readonly IUserOptionsService _userOptionsService = userOptionsService;

    private readonly LoginResult _loginFailed = new()
        { Successful = false, Error = "Username and password combination is invalid." };
    
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var newUser = new BetterExpensesUser { UserName = model.Email, Email = model.Email };

        var result = await UserManager.CreateAsync(newUser, model.Password);

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
        var user = await UserManager.FindByEmailAsync(login.Email);
        if (user == null)
        {
            return Unauthorized(_loginFailed);
        }

        if (!await UserManager.CheckPasswordAsync(user, login.Password))
        {
            return Unauthorized(_loginFailed);
        }

        var authToken = _tokenService.GenerateToken(user);
        var refreshToken = await _tokenService.GenerateRefreshToken(user);

        return Ok(new LoginResult { Successful = true, AuthToken = authToken, RefreshToken = refreshToken});
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenModel refreshModel)
    {
        var user = await UserManager.FindByIdAsync(refreshModel.UserId.ToString());
        if (user == null ||!await _tokenService.ValidateRefreshToken(refreshModel.UserId, refreshModel.RefreshToken))
        {
            return Unauthorized();
        }

        await _tokenService.InvalidateRefreshToken(refreshModel.RefreshToken);
        var authToken = _tokenService.GenerateToken(user);
        var refreshToken = await _tokenService.GenerateRefreshToken(user);
        
        return Ok(new LoginResult { Successful = true, AuthToken = authToken, RefreshToken = refreshToken});
    }

    [HttpGet]
    public async Task<IActionResult> LinkBunq()
    {
        var user = await UserManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        return Redirect(_bunqAuthService.GetAuthUri(user.Id));
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Callback(string code, string state)
    {
        if (!Guid.TryParse(state, out var stateId))
        {
            return Unauthorized("Invalid state");
        }

        if (!_bunqAuthService.TryGetUserIdForState(stateId, out var userId))
        {
            return Unauthorized("State expired or is invalid");
        }

        var accessToken = await _bunqAuthService.GetAccessToken(code);

        await Task.WhenAll(
            _apiContextService.CreateAndSaveNewApiContext(userId, accessToken),
            AddFetchAccountTask(userId),
            _userOptionsService.UpdateUserOptions(userId, new UserSettings { BunqLinked = true })
        );
        
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