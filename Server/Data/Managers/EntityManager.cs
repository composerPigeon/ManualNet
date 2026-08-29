using Microsoft.EntityFrameworkCore;
using Server.Model;
namespace Server.Data.Managers;

public interface IEntityManager<TEntity, in TKey>
    where TEntity : class, IEntityBase<TKey>
    where TKey : IEquatable<TKey>
{
    public Task<TEntity?> FindByIdAsync(TKey id);
}

public abstract class EntityManager<TEntity, TKey>(AppDbContext context) : IEntityManager<TEntity, TKey>
    where TEntity : class, IEntityBase<TKey>
    where TKey : IEquatable<TKey>
{
    protected AppDbContext Context { get; } = context;
    
    public abstract Task<TEntity?> FindByIdAsync(TKey id);
}
