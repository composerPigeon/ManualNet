using System.ComponentModel.DataAnnotations;
using Server.Data.EntityContexts;

namespace Server.Model.Domain;

public class ManufacturerEntity : IEntityBase<Guid>
{
    public Guid Id { get; init; }
    
    [MaxLength(IEntityContext.MaxNameLength)]
    public required string Name { get; init; }

    [MaxLength(IEntityContext.MaxDescriptionLength)]
    public string? Description { get; init; }
}