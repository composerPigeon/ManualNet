using System.ComponentModel.DataAnnotations;
using Server.Data.EntityContexts;
using Shared.Model.Auth;
using Shared.Model.Domain;

namespace Server.Model.Domain;

public class ManualEntity : IEntityBase<Guid>
{
    public Guid Id { get; init; }
    
    public DateTime AddedAt { get; init; }
    public Language Language { get; init; }

    public required ProductEntity Product { get; init; }
}