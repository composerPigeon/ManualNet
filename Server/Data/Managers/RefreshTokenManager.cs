using Microsoft.EntityFrameworkCore;
using Server.Model.Auth;

namespace Server.Data.Managers;

public interface IRefreshTokenManager : IDbSetEntityManager<RefreshTokenEntity>
{
    public IEnumerable<RefreshTokenEntity> FindAllActiveTokensForUser(ManualNetUserEntity userEntity);
    public Task<RefreshTokenEntity?> FindByHashAsync(string tokenHash);
}

public sealed class RefreshTokenManager(AppDbContext context)
    : DbSetEntityManager<RefreshTokenEntity>(context), IRefreshTokenManager
{
    public IEnumerable<RefreshTokenEntity> FindAllActiveTokensForUser(ManualNetUserEntity userEntity)
    {
        return Entities
            .Where(t => t.User == userEntity &&
                        t.RevokedAt == null &&
                        t.ExpiresAt > DateTime.UtcNow);
    }

    public Task<RefreshTokenEntity?> FindByHashAsync(string tokenHash)
    {
        return Entities
            .Where(t => t.TokenHash == tokenHash)
            .Include(t => t.User)
            .FirstOrDefaultAsync();
    }
}
