using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Data.Managers;
using Server.Model.Domain;
using Server.Services;
using Shared.Model.Domain;
using Shared.Responses;

namespace Server.Controllers;

[ApiController]
[Authorize]
[Route("manufacturers/")]
public class ManufacturerController(
    AppDbContext db,
    IResultFactory results,
    IManufacturerManager manufacturers) : ControllerBase
{
    [HttpGet]
    public async Task<IResult> GetAllManufacturers()
    {
        var items = manufacturers.GetAll();
        return results.List<ManufacturerListResponse, ManufacturerEntity, ManufacturerDto>(items);
    }
    
    [HttpPost]
    public async Task<IResult> CreateManufacturer(ManufacturerDto request)
    {
        var manufacturer = ManufacturerEntity.Create(request);

        manufacturers.Add(manufacturer);
        await db.SaveChangesAsync();

        return results.Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IResult> DeleteManufacturer(string id)
    {
        var manufacturer = await manufacturers.FindByIdAsync(id);
        if (manufacturer is null)
            return results.NotFound($"Manufacturer with (id: {id}) not found");

        manufacturers.Remove(manufacturer);
        await db.SaveChangesAsync();

        return results.Ok();
    }
}
