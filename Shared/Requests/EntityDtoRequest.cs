using System.Text.Json.Serialization;
using Shared.Model;
using Shared.Model.Auth;
using Shared.Model.Domain;

namespace Shared.Requests;

public abstract class EntityDtoRequest<TDto>(Token authToken) : AuthorizedRequest(authToken)
    where TDto : IEntityDto
{
    public TDto Dto { get; set; } = default!;
}

public class ManualWithRelationRequest(Token authToken) : EntityDtoRequest<ManualWithRelationDto>(authToken)
{
    public ManualWithRelationRequest() : this(default)
    {
    }

    [JsonIgnore]
    public ManualDto Manual => Dto.Manual;
    
    [JsonIgnore]
    public UserManualRelationDto Relation => Dto.Relation;
}

public class UserManualRelationRequest(Token authToken) : EntityDtoRequest<UserManualRelationDto>(authToken)
{
    public UserManualRelationRequest() : this(default)
    {
    }
}
