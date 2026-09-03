using Shared.Model;
using Shared.Model.Domain;

namespace Shared.Responses;

public abstract class ManualNetListResponseBase<TEntity> : ManualNetResponse
{
    public IEnumerable<TEntity> Items { get; set; } = [];
}

public class ManualWithRelationListResponse : ManualNetListResponseBase<ManualWithRelationDto>
{
    public override bool Success => true;
}

public class ManualListResponse : ManualNetListResponseBase<ManualDto>
{
    public override bool Success => true;
}

public class ManufacturerListResponse : ManualNetListResponseBase<ManufacturerDto>
{
    public override bool Success => true;
}

public class ProductListResponse : ManualNetListResponseBase<ProductDto>
{
    public override bool Success => true;
}
