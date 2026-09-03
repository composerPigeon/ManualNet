namespace Shared.Model.Domain;

public class ManualDto : IEntityDto
{
    public string Id { get; init; } = string.Empty;
    public Language Language { get; init; }
    public DateTime AddedAt { get; init; }
    
    public decimal AverageRating { get; init; }
    
    public string ProductId { get; init; } = string.Empty;
}
