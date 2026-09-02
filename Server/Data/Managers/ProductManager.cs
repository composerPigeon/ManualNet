using Microsoft.EntityFrameworkCore;
using Server.Model.Domain;
using Shared.Model.Domain;

namespace Server.Data.Managers;

public interface IProductManager : IDbSetEntityManager<ProductEntity, Guid>
{
    
}

public sealed class ProductManager(AppDbContext context) : DbSetEntityManager<ProductEntity, Guid>(context), IProductManager
{
    
}