using Server.Model.Domain;

namespace Server.Data.Managers;

public interface IUserManualRelationManager : IDbSetEntityManager<UserManualRelation, Guid>
{
    
}

public class UserManualRelationManager(AppDbContext context) : DbSetEntityManager<UserManualRelation, Guid>(context), IUserManualRelationManager
{
    
}