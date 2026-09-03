namespace Shared.Model.Domain;

public class ProductDto : IEntityDto
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    
    public string? Description { get; init; }
    
    public string ManufacturerId { get; init; } = string.Empty;
}
