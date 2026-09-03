using System.ComponentModel.DataAnnotations;
using Server.Data.EntityContexts;
using Server.Model;

namespace Server.Model.Auth;

public class RefreshTokenEntity : IEntityBase
{
    public string Id { get; private init; } = string.Empty;

    [MaxLength(RefreshTokenEntityContext.TokenHashMaxLength)]
    public string TokenHash { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime ExpiresAt { get; init; }
    public DateTime? RevokedAt { get; set; }

    public ManualNetUserEntity User { get; init; } = null!;

    public bool IsActive => RevokedAt == null && ExpiresAt > DateTime.UtcNow;

    public static RefreshTokenEntity Create(ManualNetUserEntity userEntity, HashToken token)
    {
        return new RefreshTokenEntity
        {
            TokenHash = token.Hash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = token.ExpiresAt,
            User = userEntity,
        };
    }
}
