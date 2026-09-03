using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Model.Auth;

namespace Server.Controllers;

[ApiController]
[Authorize]
[Route("/profile")]
public class ProfileController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok($"Hello {User.Identity?.Name}, you reached a protected endpoint.");
    }

    [HttpGet("admin")]
    [Authorize(Roles = Roles.AdminRoleName)]
    public IActionResult GetAdmin()
    {
        return Ok("Hello Admin, this endpoint is for administrators only.");
    }
}
