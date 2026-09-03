using Shared.Model;

namespace Server.Model.Domain;

public interface IDtoEntity
{
    public static TDtoEntity CreateFrom<TDtoEntity, TDto>(TDto dto)
        where TDtoEntity : DtoEntityBase<TDto>, new()
        where TDto : IEntityDto<Guid>
    {
        var entity = new TDtoEntity();
        entity.InitDataFrom(dto);
        return entity;
    }
}

public abstract class DtoEntityBase<TDto>() : IEntityBase<Guid>, IDtoEntity
    where  TDto : IEntityDto<Guid>
{
    public Guid Id { get; protected set; }
    
    public abstract TDto AsDto();
    public abstract void InitDataFrom(TDto dto);
}