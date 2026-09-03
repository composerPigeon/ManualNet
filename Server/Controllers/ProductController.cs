using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Data.Managers;
using Server.Model.Domain;
using Server.Services;
using Shared.Model.Domain;

namespace Server.Controllers;

[ApiController]
[Authorize]
[Route("products/")]
public class ProductController(
    AppDbContext db,
    IResultFactory results,
    IProductManager products,
    IManufacturerManager manufacturers) : ControllerBase
{
    [HttpPost]
    public async Task<IResult> CreateProduct(ProductDto request)
    {
        var manufacturer = await manufacturers.FindByIdAsync(request.ManufacturerId);
        if (manufacturer is null)
        {
            return results.NotFound(
                $"Manufacturer with (id: {request.ManufacturerId}) not found");
        }

        var product = ProductEntity.Create(request, manufacturer);

        products.Add(product);
        await db.SaveChangesAsync();

        return results.Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IResult> DeleteProduct(string id)
    {
        var product = await products.FindByIdAsync(id);
        if (product is null)
            return results.NotFound($"Product with (id: {id}) not found");

        products.Remove(product);
        await db.SaveChangesAsync();

        return results.Ok();
    }
}
