using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Model.Auth;

namespace Server.Controllers;

[ApiController]
[Authorize]
[Route("api/secured")]
public class SecuredController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok($"Hello {User.Identity?.Name}, you reached a protected endpoint.");
    }

    [HttpGet("admin")]
    [Authorize(Roles = Roles.Admin)]
    public IActionResult GetAdmin()
    {
        return Ok("Hello Admin, this endpoint is for administrators only.");
    }
}
