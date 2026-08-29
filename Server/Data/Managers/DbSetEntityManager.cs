using Microsoft.EntityFrameworkCore;
using Server.Model;

namespace Server.Data.Managers;

public interface IDbSetEntityManager<TEntity, in TKey> : IEntityManager<TEntity, TKey>
    where TEntity : class, IEntityBase<TKey>
    where TKey : IEquatable<TKey>
{
    public void Add(TEntity entity);
}

public abstract class DbSetEntityManager<TEntity, TKey>(AppDbContext context) : EntityManager<TEntity, TKey>(context), IDbSetEntityManager<TEntity, TKey>
    where TEntity : class, IEntityBase<TKey>
    where TKey : IEquatable<TKey>
{
    protected abstract DbSet<TEntity> Entities { get; }
    
    public override Task<TEntity?> FindByIdAsync(TKey id)
    {
        return Entities.Where(e => e.Id.Equals(id)).SingleOrDefaultAsync();
    }

    public void Add(TEntity entity)
    {
        Entities.Add(entity);
    }
}