using System.ComponentModel.DataAnnotations;
using Server.Data.EntityContexts;
using Shared.Model.Domain;

namespace Server.Model.Domain;

public class ManufacturerEntity : DtoEntityBase<ManufacturerDto>
{
    [MaxLength(IEntityContext.MaxNameLength)]
    public string Name { get; init; }

    [MaxLength(IEntityContext.MaxDescriptionLength)]
    public string? Description { get; init; }

    public override ManufacturerDto AsDto()
    {
        return new ManufacturerDto
        {
            Id = Id
        };
    }

    public override void InitDataFrom(ManufacturerDto dto)
    {
        throw new NotImplementedException();
    }
}
