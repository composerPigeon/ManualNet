namespace Shared.Model.Domain;

public class ManufacturerDto : IEntityDto<Guid>
{
    public Guid Id { get; init; }
}