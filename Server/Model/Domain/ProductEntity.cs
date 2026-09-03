using System.ComponentModel.DataAnnotations;
using Server.Data.EntityContexts;
using Shared.Model.Domain;

namespace Server.Model.Domain;

public class ProductEntity : DtoEntityBase<ProductDto>
{
    [MaxLength(IEntityContext.MaxNameLength)]
    public string Name { get; private set; }
    
    [MaxLength(IEntityContext.MaxDescriptionLength)]
    public string? Description { get; private set; }
    
    public ManufacturerEntity Manufacturer { get; private set; }

    public override ProductDto AsDto()
    {
        return new ProductDto
        {
            Id = Id,
            Name = Name,
            Description = Description ?? string.Empty,
            Manufacturer = Manufacturer.AsDto()
        };
    }

    public override void InitDataFrom(ProductDto dto)
    {
        Id =  dto.Id;
        Name = dto.Name;
        Description = dto.Description;
        Manufacturer = IDtoEntity.CreateFrom<ManufacturerEntity,ManufacturerDto>(dto.Manufacturer);
    }
}
