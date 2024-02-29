using AutoMapper;
using BetterExpenses.API.Exceptions;
using BetterExpenses.Common.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BetterExpenses.API.Controllers;

/// <summary>
/// Default controller for api endpoints that are authorized.
/// </summary>
[ApiController, Authorize]
[Route("api/[controller]/[action]")]
public abstract class AuthorizedApiControllerBase : ControllerBase
{
    private UserManager<BetterExpensesUser>? _userManager;
    
    protected UserManager<BetterExpensesUser> UserManager
    {
        get
        {
            return _userManager ??= HttpContext.RequestServices.GetRequiredService<UserManager<BetterExpensesUser>>();
        }
    }
    
    private IMapper? _mapper;
    
    protected IMapper Mapper
    {
        get
        {
            return _mapper ??= HttpContext.RequestServices.GetRequiredService<IMapper>();
        }
    }

    protected async Task<BetterExpensesUser> GetUser()
    {
        var user = await UserManager.GetUserAsync(HttpContext.User);
        if (user == null)
        {
            throw new ApiAuthenticationException(HttpContext.Request.GetDisplayUrl());
        }
        
        return user;
    } 
}