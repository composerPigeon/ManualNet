namespace Shared.Model.Domain;

public class ManufacturerDto : IEntityDto
{
    public string Id { get; init; } = string.Empty;
    
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}
