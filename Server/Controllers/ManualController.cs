using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Server.Data.Managers;
using Server.Services;

namespace Server.Controllers;

[Authorize]
[Route("manuals/")]
public class ManualController(
    IResultFactory results,
    IManualNetUserManager users,
    IManualManager manuals,
    IUserManualRelationManager userManualRelations) : ControllerBase
{
    [HttpGet]
    public IResult GetAllManualsForUser()
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
            return results.Unauthorized();

        var relations = userManualRelations.FindRelsForUser(userId);
        
        return results.Manuals(relations);
    }
    
    [HttpPost]
    public IResult CreateManual(object request)
    {
        return results.Ok();
    }

    [HttpDelete("/{id}")]
    public IResult DeleteManual(Guid id)
    {
        
        
        return results.Ok();
    }
}