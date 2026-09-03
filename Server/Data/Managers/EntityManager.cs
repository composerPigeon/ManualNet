using Server.Model;

namespace Server.Data.Managers;

public interface IEntityManager;

public interface IEntityManager<TEntity> : IEntityManager
    where TEntity : class, IEntityBase
{
    public Task<TEntity?> FindByIdAsync(string id);
}

public abstract class EntityManager<TEntity>(AppDbContext context) : IEntityManager<TEntity>
    where TEntity : class, IEntityBase
{
    protected AppDbContext Context { get; } = context;
    
    public abstract Task<TEntity?> FindByIdAsync(string id);
}
