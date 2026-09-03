using Microsoft.EntityFrameworkCore;
using Server.Model.Domain;

namespace Server.Data.Managers;

public interface IManualManager : IDbSetEntityManager<ManualEntity>
{
    public IEnumerable<ManualEntity> GetAllByProduct(ProductEntity product);
    
    public IEnumerable<ManualEntity> GetAllByManufacturer(ManufacturerEntity manufacturer);
}

public sealed class ManualManager(AppDbContext context) : DbSetEntityManager<ManualEntity>(context), IManualManager
{
    public IEnumerable<ManualEntity> GetAllByProduct(ProductEntity product)
    {
        return Entities.Where(m => m.Product.Id == product.Id);
    }

    public IEnumerable<ManualEntity> GetAllByManufacturer(ManufacturerEntity manufacturer)
    {
        return Entities
            .Where(m => m.Product.Manufacturer.Id == manufacturer.Id);
    }
}
