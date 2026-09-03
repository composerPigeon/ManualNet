namespace Shared.Responses;

public class NewEntityResponse<TKey> : ManualNetResponse
    where TKey : IEquatable<TKey>
{
    public override bool Success => true;
    
    public required TKey Id { get; set; }
}