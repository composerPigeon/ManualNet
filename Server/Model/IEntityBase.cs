namespace Server.Model;

public interface IEntityBase<out TKey>
    where TKey : IEquatable<TKey>
{
    public TKey Id { get; }
}