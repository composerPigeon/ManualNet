using Microsoft.EntityFrameworkCore;
using Server.Model.Domain;

namespace Server.Data.Managers;

public interface IUserManualRelationManager : IDbSetEntityManager<UserManualRelation, Guid>
{
    IEnumerable<UserManualRelation> FindRelsForUser(string userId);
    
    Task<UserManualRelation> FindRelForUserAndManualAsync(string userId, Guid manualId);
}

public class UserManualRelationManager(AppDbContext context) : DbSetEntityManager<UserManualRelation, Guid>(context), IUserManualRelationManager
{
    public IEnumerable<UserManualRelation> FindRelsForUser(string userId)
    {
        return Entities
            .Include(r => r.Manual)
                .ThenInclude(m => m.Product)
                    .ThenInclude(p => p.Manufacturer)
            .Where(r => r.User.Id == userId);
    }

    public async Task<UserManualRelation> FindRelForUserAndManualAsync(string userId, Guid manualId)
    {
        var relation = await Entities
            .Include(r => r.Manual)
                .ThenInclude(m => m.Product)
                    .ThenInclude(p => p.Manufacturer)
            .SingleAsync(r => r.User.Id == userId && r.Manual.Id == manualId);

        return relation;
    }
}
