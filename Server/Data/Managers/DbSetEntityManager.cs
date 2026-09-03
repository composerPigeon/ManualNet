using Microsoft.EntityFrameworkCore;
using Server.Model;

namespace Server.Data.Managers;

public interface IDbSetEntityManager<TEntity> : IEntityManager<TEntity>
    where TEntity : class, IEntityBase
{
    public IEnumerable<TEntity> GetAll();
    public void Add(TEntity entity);
    public void Remove(TEntity entity);
}

public abstract class DbSetEntityManager<TEntity>(AppDbContext context)
    : EntityManager<TEntity>(context), IDbSetEntityManager<TEntity>
    where TEntity : class, IEntityBase
{
    protected DbSet<TEntity> Entities => Context.Set<TEntity>();

    public override Task<TEntity?> FindByIdAsync(string id)
    {
        return Entities.Where(e => e.Id.Equals(id)).SingleOrDefaultAsync();
    }

    public IEnumerable<TEntity> GetAll()
    {
        return Entities;
    }

    public void Add(TEntity entity)
    {
        Entities.Add(entity);
    }

    public void Remove(TEntity entity)
    {
        Entities.Remove(entity);
    }
}
