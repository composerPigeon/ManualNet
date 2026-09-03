using Server.Model.Domain;

namespace Server.Data.Managers;

public interface IManufacturerManager : IDbSetEntityManager<ManufacturerEntity>;

public class ManufacturerManager(AppDbContext context) : DbSetEntityManager<ManufacturerEntity>(context), IManufacturerManager
{
    
}
