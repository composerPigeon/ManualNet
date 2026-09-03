using System.ComponentModel.DataAnnotations;
using Server.Data.EntityContexts;
using Server.Model.Auth;
using Shared.Model.Domain;

namespace Server.Model.Domain;

public class UserManualRelation : DtoEntityBase<ManualDto>
{
    public ManualRating Rating { get; private set; }
    
    [MaxLength(IEntityContext.MaxNameLength)]
    public required string LocalFileName { get; init; }
    
    public required ManualNetUserEntity User { get; init; }
    public required ManualEntity Manual { get; init; }

    public void UpdateRating(ManualRating rating)
    {
        Rating = rating;
    }

    public override ManualDto AsDto()
    {
        return new ManualDto
        {
            Id = Manual.Id,
            AddedAt = Manual.AddedAt,
            Language = Manual.Language,

            FileName = LocalFileName,
            Rating = Rating,
            Product =  Manual.Product.AsDto(),
        };
    }

    public override void InitDataFrom(ManualDto dto)
    {
        throw new NotImplementedException();
    }
}
