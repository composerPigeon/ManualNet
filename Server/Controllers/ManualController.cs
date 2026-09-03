using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Server.Data;
using Server.Data.Managers;
using Server.Model.Domain;
using Server.Services;
using Shared.Model.Domain;
using Shared.Requests;
using Shared.Responses;

namespace Server.Controllers;

[ApiController]
[Authorize]
[Route("manuals/")]
public class ManualController(
    AppDbContext db,
    IResultFactory results,
    IManualNetUserManager users,
    IManualManager manuals,
    IProductManager products,
    IUserManualRelationManager userManualRelations) : ControllerBase
{
    [HttpGet]
    public IResult GetAllManualsForUser()
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
            return results.Unauthorized();

        var relations = userManualRelations.FindRelsForUser(userId);

        return results.List<ManualWithRelationListResponse, UserManualRelation, ManualWithRelationDto>(relations);
    }

    [HttpGet]
    public IResult GetAllManualsForUserAndManufacturer([FromQuery]string manufacturerId)
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return results.Unauthorized();

        var result = userManualRelations.FindManualsForUserAndManufacturer(userId, manufacturerId);

        return results.List<ManualListResponse, ManualEntity, ManualDto>(result);
    }

    [HttpPost]
    public async Task<IResult> CreateManual(ManualWithRelationRequest request)
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return results.Unauthorized();

        var user = await users.FindByIdAsync(userId);
        if (user is null)
            return results.Unauthorized();

        var productId = request.Manual.ProductId;
        var product = await products.FindByIdAsync(productId);

        if (product is null)
            return results.NotFound($"Product with (id: {productId}) not found");

        var newManual = ManualEntity.Create(request.Manual, product);
        var newRelation = UserManualRelation.Create(request.Relation, newManual, user);

        manuals.Add(newManual);
        userManualRelations.Add(newRelation);

        await db.SaveChangesAsync();
        return results.Ok();
    }

    [HttpPost("register")]
    public async Task<IResult> RegisterManual(UserManualRelationRequest request)
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return results.Unauthorized();
        
        var user = await users.FindByIdAsync(userId);
        if (user is null)
            return results.Unauthorized();
        
        var manual = await manuals.FindByIdAsync(request.Dto.ManualId);
        if (manual is null)
            return results.NotFound($"Manual with (id: {request.Dto.ManualId}) not found");

        var relation = UserManualRelation.Create(request.Dto, manual, user);
        userManualRelations.Add(relation);
        await db.SaveChangesAsync();
        return results.Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IResult> DeleteManual(string id)
    {
        var entity = await manuals.FindByIdAsync(id);
        if (entity is null)
            return results.NotFound($"Manual with (id: {id}) not found");
        
        manuals.Remove(entity);
        await db.SaveChangesAsync();
        return results.Ok();
    }
}
