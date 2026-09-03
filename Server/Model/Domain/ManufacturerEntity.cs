using System.ComponentModel.DataAnnotations;
using Server.Data.EntityContexts;
using Shared.Model.Domain;

namespace Server.Model.Domain;

public class ManufacturerEntity : IDtoEntity<ManufacturerDto>
{
    public string Id { get; private set; } = string.Empty;

    [MaxLength(IEntityContext.MaxNameLength)]
    public string Name { get; init; }

    [MaxLength(IEntityContext.MaxDescriptionLength)]
    public string? Description { get; init; }

    public ManufacturerDto AsDto()
    {
        return new ManufacturerDto
        {
            Id = Id,
            Name = Name,
            Description = Description
        };
    }

    public static ManufacturerEntity Create(ManufacturerDto dto)
    {
        return new ManufacturerEntity
        {
            Name = dto.Name,
            Description = dto.Description
        };
    }
}
