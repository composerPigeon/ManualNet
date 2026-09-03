using Microsoft.EntityFrameworkCore;
using Server.Model.Domain;
using Shared.Model.Domain;

namespace Server.Data.Managers;

public interface IProductManager : IDbSetEntityManager<ProductEntity>
{
    
}

public sealed class ProductManager(AppDbContext context) : DbSetEntityManager<ProductEntity>(context), IProductManager
{
    
}
