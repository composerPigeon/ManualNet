using Server.Model;
using Server.Model.Domain;

namespace Server.Data.EntityContexts;

public interface IEntityContextCollection
{
    public void RegisterContext<TEntity>(Func<EntityContextBase<TEntity>> ctor)
        where TEntity : class, IEntityBase;
    
    public IEnumerable<IEntityContext> GetAllContexts();
}

public class EntityContextCollection : IEntityContextCollection
{
    private readonly Dictionary<Type, Func<IEntityContext>> _contexts = new ();

    public void RegisterContext<TEntity>(Func<EntityContextBase<TEntity>> ctor)
        where TEntity : class, IEntityBase
    {
        _contexts.Add(typeof(TEntity), ctor);
    }
    
    public IEnumerable<IEntityContext> GetAllContexts()
    {
        foreach (var ctor in _contexts.Values)
        {
            yield return ctor.Invoke();
        }
    }

    public static IEntityContextCollection Default => InitializeDefault();

    private static EntityContextCollection InitializeDefault()
    {
        var collection = new EntityContextCollection();
        
        collection.RegisterContext(() => new ProductEntityContext());
        collection.RegisterContext(() => new ManualEntityContext());
        collection.RegisterContext(() => new ManufacturerEntityContext());
        collection.RegisterContext(() => new ManualNetUserEntityContext());
        collection.RegisterContext(() => new RefreshTokenEntityContext());
        collection.RegisterContext(() => new UserManualRelationContext());
        return collection;
    }
}
