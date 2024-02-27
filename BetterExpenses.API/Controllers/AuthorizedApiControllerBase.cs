using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterExpenses.API.Controllers;

/// <summary>
/// Default controller for api endpoints that are authorized.
/// </summary>
[ApiController, Authorize]
[Route("api/[controller]/[action]")]
public abstract class AuthorizedApiControllerBase : ControllerBase
{
    
}