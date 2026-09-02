using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Model;
namespace Server.Data.EntityContexts;

public interface IEntityContext
{
    public const int MaxNameLength = 128;
    public const int MaxDescriptionLength = 1024;
    
    public void MapBuilder(ModelBuilder builder);
}

public abstract class EntityContextBase<TEntity, TKey> : IEntityContext
    where TEntity : class, IEntityBase<TKey>
    where TKey : IEquatable<TKey>
{
    public void MapBuilder(ModelBuilder builder)
    {
        builder.Entity<TEntity>(MapProperties);
    }

    protected virtual void MapProperties(EntityTypeBuilder<TEntity> entity)
    {
        entity.HasKey(e => e.Id);
    }
}