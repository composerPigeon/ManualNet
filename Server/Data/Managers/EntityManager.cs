using Microsoft.EntityFrameworkCore;
using Server.Model;
namespace Server.Data.Managers;

public interface IEntityManager<TEntity, in TKey>
    where TEntity : class, IEntityBase<TKey>
    where TKey : IEquatable<TKey>
{
    public Task<TEntity> GetByIdAsync(TKey id);
    
    public void Add(TEntity entity);
}

public abstract class EntityManager<TEntity, TKey>(AppDbContext context) : IEntityManager<TEntity, TKey>
    where TEntity : class, IEntityBase<TKey>
    where TKey : IEquatable<TKey>
{
    protected abstract DbSet<TEntity> Entities { get; }
    protected AppDbContext Context { get; } = context;

    public Task<TEntity> GetByIdAsync(TKey id)
    {
        return Entities.Where(e => e.Id.Equals(id)).SingleAsync();
    }

    public void Add(TEntity entity)
    {
        Entities.Add(entity);
    }
}
