namespace Shared.Model.Domain;

public class ManualDto : IEntityDto<Guid>
{
    public Guid Id { get; init; }
    public Language Language { get; init; }
    public DateTime AddedAt { get; init; }
    
    public ManualRating Rating { get; init; }
    public string FileName { get; init; } = string.Empty;
    
    public ProductDto Product { get; init; }
}