namespace Shared.Model.Domain;

public class UserManualRelationDto : IEntityDto
{
    public string Id { get; init; } = string.Empty;
    
    public string ManualId { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    
    public string LocalFileName { get; init; } = string.Empty;
    public ManualRating Rating { get; init; }
}
