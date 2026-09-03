namespace Shared.Model;

public interface IEntityDto<TKey>
    where TKey : IEquatable<TKey>
{
    public TKey Id { get; init; }
}