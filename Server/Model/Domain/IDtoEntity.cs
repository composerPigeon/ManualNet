using Shared.Model;

namespace Server.Model.Domain;

public interface IDtoEntity<TDto> : IEntityBase
    where TDto : IEntityDto
{
    public TDto AsDto();
}