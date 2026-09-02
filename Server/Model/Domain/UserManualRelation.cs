using System.ComponentModel.DataAnnotations;
using Server.Data.EntityContexts;
using Server.Model.Auth;
using Shared.Model.Domain;

namespace Server.Model.Domain;

public class UserManualRelation : IEntityBase<Guid>
{
    public Guid Id { get; init; }
    
    public ManualRating Rating { get; private set; }
    
    [MaxLength(IEntityContext.MaxNameLength)]
    public required string LocalFileName { get; init; }
    
    public required ManualNetUserEntity User { get; init; }
    public required ManualEntity Manual { get; init; }

    public void UpdateRating(ManualRating rating)
    {
        Rating = rating;
    }
}