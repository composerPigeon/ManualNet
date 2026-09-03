using System.ComponentModel.DataAnnotations;
using Server.Data;
using Server.Data.EntityContexts;
using Server.Model.Auth;
using Shared.Model.Domain;

namespace Server.Model.Domain;

public class UserManualRelation : IDtoEntity<UserManualRelationDto>, IDtoEntity<ManualWithRelationDto>
{
    public string Id { get; private set; } = string.Empty;
    public ManualRating Rating { get; private set; }
    
    [MaxLength(IEntityContext.MaxNameLength)]
    public string LocalFileName { get; private set; } = string.Empty;
    
    public ManualNetUserEntity User { get; private set; }
    
    public ManualEntity Manual { get; private set; }

    public void UpdateRating(ManualRating rating)
    {
        Rating = rating;
    }

    public UserManualRelationDto AsDto()
    {
        return new UserManualRelationDto
        {
            Id = Id,
            LocalFileName = LocalFileName,
            Rating = Rating,
            UserId = User.Id,
            ManualId = Manual.Id
        };
    }

    ManualWithRelationDto IDtoEntity<ManualWithRelationDto>.AsDto()
    {
        return new ManualWithRelationDto
        {
            Relation = AsDto(),
            Manual = Manual.AsDto()
        };
    }

    public static UserManualRelation Create(UserManualRelationDto dto, ManualEntity manual, ManualNetUserEntity user)
    {
        return new UserManualRelation
        {
            LocalFileName = dto.LocalFileName,
            Rating = dto.Rating,
            Manual = manual,
            User = user
        };
    }
}
