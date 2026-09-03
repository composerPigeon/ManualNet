using Shared.Model.Domain;

namespace Shared.Responses;

public class ManualListResponse : ManualNetResponse
{
    public override bool Success => true;

    public IEnumerable<ManualDto> Manuals { get; init; } = [];
}