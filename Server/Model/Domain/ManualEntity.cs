using Shared.Model.Domain;

namespace Server.Model.Domain;

public class ManualEntity : DtoEntityBase<ManualDto>
{
    public DateTime AddedAt { get; private set; }
    public Language Language { get; private set; }

    public ProductEntity Product { get; private set; }

    public override ManualDto AsDto()
    {
        return new ManualDto
        {
            Id = Id,
            AddedAt = AddedAt,
            Language = Language,
            FileName = string.Empty,
            Rating = default,
            Product = Product.AsDto()
        };
    }

    public override void InitDataFrom(ManualDto dto)
    {
        AddedAt = dto.AddedAt;
        Language = dto.Language;
        Product = IDtoEntity.CreateFrom<ProductEntity,ProductDto>(dto.Product);
    }
}
