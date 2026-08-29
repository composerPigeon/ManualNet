using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Server.Model.Auth;
using Shared.Model.Auth;

namespace Server.Data.Managers;

public interface IManualNetUserManager : IEntityManager<ManualNetUserEntity, string>
{
    public Task<ManualNetUserEntity?> FindByEmailAsync(Email email);
    
    public Task<IdentityResult> CreateAsync(ManualNetUserEntity user, string password);
    public Task<IdentityResult> AddToRoleAsync(ManualNetUserEntity user, Role role);
    public Task<bool> CheckPasswordAsync(ManualNetUserEntity user, string password);
    public Task<IList<Role>> GetRolesAsync(ManualNetUserEntity user);
}

public class ManualNetUserManager(
    IUserStore<ManualNetUserEntity> store,
    IOptions<IdentityOptions> optionsAccessor,
    IPasswordHasher<ManualNetUserEntity> passwordHasher,
    IEnumerable<IUserValidator<ManualNetUserEntity>> userValidators,
    IEnumerable<IPasswordValidator<ManualNetUserEntity>> passwordValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    IServiceProvider services,
    ILogger<UserManager<ManualNetUserEntity>> logger)
    : UserManager<ManualNetUserEntity>(
        store,
        optionsAccessor,
        passwordHasher,
        userValidators,
        passwordValidators,
        keyNormalizer,
        errors,
        services,
        logger), IManualNetUserManager
{
    public Task<ManualNetUserEntity?> FindByEmailAsync(Email email)
    {
        return base.FindByEmailAsync(email.ToString());
    }

    public new async Task<IList<Role>> GetRolesAsync(ManualNetUserEntity user)
    {
        var roleNames = await base.GetRolesAsync(user);
        return roleNames.Select(Role.FromName).ToList();
    }

    public async Task<IdentityResult> AddToRoleAsync(ManualNetUserEntity user, Role role)
    {
        return await base.AddToRoleAsync(user, role.Name);
    }
}
