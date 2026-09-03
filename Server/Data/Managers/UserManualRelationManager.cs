using Microsoft.EntityFrameworkCore;
using Server.Model.Domain;

namespace Server.Data.Managers;

public interface IUserManualRelationManager : IDbSetEntityManager<UserManualRelation>
{
    IEnumerable<UserManualRelation> FindRelsForUser(string userId);
    
    IEnumerable<ManualEntity> FindManualsForUserAndManufacturer(string userId, string manufacturerId);
    
    Task<UserManualRelation> FindRelForUserAndManualAsync(string userId, string manualId);
}

public class UserManualRelationManager(AppDbContext context) : DbSetEntityManager<UserManualRelation>(context), IUserManualRelationManager
{
    public IEnumerable<UserManualRelation> FindRelsForUser(string userId)
    {
        return Entities
            .Include(r => r.Manual)
            .Where(r => r.User.Id == userId);
    }

    public async Task<UserManualRelation> FindRelForUserAndManualAsync(string userId, string manualId)
    {
        var relation = await Entities
            .Include(r => r.Manual)
            .SingleAsync(r => r.User.Id == userId && r.Manual.Id == manualId);

        return relation;
    }

    public IEnumerable<ManualEntity> FindManualsForUserAndManufacturer(string userId, string manufacturerId)
    {
        return Entities
            .Include(r => r.Manual)
            .Where(r => r.User.Id == userId && r.Manual.Product.Manufacturer.Id == manufacturerId)
            .Select(r => r.Manual);
    }
}
