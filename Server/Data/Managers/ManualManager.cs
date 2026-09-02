using Microsoft.EntityFrameworkCore;
using Server.Model.Domain;

namespace Server.Data.Managers;

public interface IManualManager : IDbSetEntityManager<ManualEntity, Guid>
{
    public IEnumerable<ManualEntity> GetAll();
    
    public IEnumerable<ManualEntity> GetAllByProduct(ProductEntity product);
    
    public IEnumerable<ManualEntity> GetAllByManufacturer(ManufacturerEntity manufacturer);
}

public sealed class ManualManager(AppDbContext context) : DbSetEntityManager<ManualEntity, Guid>(context), IManualManager
{
    public IEnumerable<ManualEntity> GetAll()
    {
        return Entities;
    }

    public IEnumerable<ManualEntity> GetAllByProduct(ProductEntity product)
    {
        return Entities.Where(m => m.Product.Id == product.Id);
    }

    public IEnumerable<ManualEntity> GetAllByManufacturer(ManufacturerEntity manufacturer)
    {
        return Entities.Where(m => m.Product.Manufacturer.Id == manufacturer.Id);
    }
}