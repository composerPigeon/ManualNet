using Microsoft.EntityFrameworkCore;
using Server.Model.Auth;

namespace Server.Data.Managers;

public interface IRefreshTokenManager : IEntityManager<RefreshTokenEntity, Guid>
{
    public Task<ICollection<RefreshTokenEntity>> GetAllActiveTokensForUserAsync(ApplicationUser user);
    public Task<RefreshTokenEntity?> GetByHashAsync(string tokenHash);
}

public class RefreshTokenManager(AppDbContext context)
    : EntityManager<RefreshTokenEntity, Guid>(context), IRefreshTokenManager
{
    protected override DbSet<RefreshTokenEntity> Entities => Context.RefreshTokens;
    
    public async Task<ICollection<RefreshTokenEntity>> GetAllActiveTokensForUserAsync(ApplicationUser user)
    {
        return await Entities
            .Where(t => t.User == user &&
                        t.RevokedAt == null &&
                        t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<RefreshTokenEntity?> GetByHashAsync(string tokenHash)
    {
        return await Entities
            .Where(t => t.TokenHash == tokenHash)
            .Include(t => t.User)
            .FirstOrDefaultAsync();
    }
}
