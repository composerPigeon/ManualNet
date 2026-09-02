using Server.Model.Domain;

namespace Server.Data.Managers;

public interface IManufacturerManager : IDbSetEntityManager<ManufacturerEntity, Guid>;

public class ManufacturerManager(AppDbContext context) : DbSetEntityManager<ManufacturerEntity, Guid>(context), IManufacturerManager
{
    
}