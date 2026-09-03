namespace Shared.Model.Domain;

public class ProductDto : IEntityDto<Guid>
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    
    public string Description { get; init; }
    
    public ManufacturerDto Manufacturer { get; init; }
}