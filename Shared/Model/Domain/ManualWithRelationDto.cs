namespace Shared.Model.Domain;

public class ManualWithRelationDto : IEntityDto
{
    public ManualDto Manual { get; init; }
    public UserManualRelationDto Relation { get; init; }
}