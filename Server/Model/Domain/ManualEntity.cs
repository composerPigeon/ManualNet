using Server.Data;
using Shared.Model.Domain;

namespace Server.Model.Domain;

public class ManualEntity : IDtoEntity<ManualDto>
{
    public string Id { get; private set; } = string.Empty;
    public DateTime AddedAt { get; private set; }
    public Language Language { get; private set; }
    public ProductEntity Product { get; private set; }

    public ManualDto AsDto()
    {
        return new ManualDto
        {
            Id = Id,
            AddedAt = AddedAt,
            Language = Language,
            ProductId = Product.Id,
        };
    }

    public static ManualEntity Create(ManualDto dto, ProductEntity product)
    {
        return new ManualEntity
        {
            AddedAt = DateTime.UtcNow,
            Language = dto.Language,
            Product = product,
        };
    }
}
