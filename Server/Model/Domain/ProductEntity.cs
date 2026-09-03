using System.ComponentModel.DataAnnotations;
using Server.Data;
using Server.Data.EntityContexts;
using Shared.Model.Domain;

namespace Server.Model.Domain;

public class ProductEntity : IDtoEntity<ProductDto>
{
    public string Id { get; private set; } = string.Empty;

    [MaxLength(IEntityContext.MaxNameLength)]
    public string Name { get; private set; } = string.Empty;
    
    [MaxLength(IEntityContext.MaxDescriptionLength)]
    public string? Description { get; private set; }
    
    public ManufacturerEntity Manufacturer { get; private set; }

    public ProductDto AsDto()
    {
        return new ProductDto
        {
            Id = Id,
            Name = Name,
            Description = Description ?? string.Empty,
            ManufacturerId = Manufacturer.Id,
        };
    }

    public static ProductEntity Create(ProductDto dto, ManufacturerEntity manufacturer)
    {
        return new ProductEntity
        {
            Name = dto.Name,
            Description = dto.Description,
            Manufacturer = manufacturer,
        };
    }
}
