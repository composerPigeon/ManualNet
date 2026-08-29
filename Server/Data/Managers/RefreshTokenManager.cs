using Microsoft.EntityFrameworkCore;
using Server.Model.Auth;

namespace Server.Data.Managers;

public interface IRefreshTokenManager : IDbSetEntityManager<RefreshTokenEntity, Guid>
{
    public Task<ICollection<RefreshTokenEntity>> FindAllActiveTokensForUserAsync(ManualNetUserEntity userEntity);
    public Task<RefreshTokenEntity?> FindByHashAsync(string tokenHash);
}

public class RefreshTokenManager(AppDbContext context)
    : DbSetEntityManager<RefreshTokenEntity, Guid>(context), IRefreshTokenManager
{
    protected override DbSet<RefreshTokenEntity> Entities => Context.RefreshTokens;
    
    public async Task<ICollection<RefreshTokenEntity>> FindAllActiveTokensForUserAsync(ManualNetUserEntity userEntity)
    {
        return await Entities
            .Where(t => t.UserEntity == userEntity &&
                        t.RevokedAt == null &&
                        t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<RefreshTokenEntity?> FindByHashAsync(string tokenHash)
    {
        return await Entities
            .Where(t => t.TokenHash == tokenHash)
            .Include(t => t.UserEntity)
            .FirstOrDefaultAsync();
    }
}
