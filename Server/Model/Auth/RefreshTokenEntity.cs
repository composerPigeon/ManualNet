using System.ComponentModel.DataAnnotations;
using Server.Data.EntityContexts;

namespace Server.Model.Auth;

public class RefreshTokenEntity : IEntityBase<Guid>
{
    public Guid Id { get; init; }

    [MaxLength(RefreshTokenEntityContext.TokenHashMaxLength)]
    public string TokenHash { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime ExpiresAt { get; init; }
    public DateTime? RevokedAt { get; set; }

    public ApplicationUser User { get; init; } = null!;

    public bool IsActive => RevokedAt == null && ExpiresAt > DateTime.UtcNow;

    public static RefreshTokenEntity From(ApplicationUser user, HashToken token)
    {
        return new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            TokenHash = token.Hash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = token.ExpiresAt,
            User = user,
        };
    }
}
