using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Server.Model.Auth;
using Shared.Model.Auth;

namespace Server.Data.Managers;

public interface IManualNetUserManager : IEntityManager<ManualNetUserEntity, string>
{
    public Task<ManualNetUserEntity?> FindByEmailAsync(ManualNetEmail email);
    
    public Task<IdentityResult> CreateAsync(ManualNetUserEntity user, Password password);
    public Task<IdentityResult> AddToRoleAsync(ManualNetUserEntity user, Role role);
    public Task<bool> IsInRoleAsync(ManualNetUserEntity user, Role role);
    public Task<bool> CheckPasswordAsync(ManualNetUserEntity user, Password password);
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
    public Task<ManualNetUserEntity?> FindByEmailAsync(ManualNetEmail email)
    {
        return base.FindByEmailAsync(email.ToString());
    }

    public new async Task<IList<Role>> GetRolesAsync(ManualNetUserEntity user)
    {
        var roleNames = await base.GetRolesAsync(user);
        return roleNames.Select(Role.FromName).ToList();
    }

    public Task<IdentityResult> AddToRoleAsync(ManualNetUserEntity user, Role role)
    {
        return base.AddToRoleAsync(user, role.Name);
    }

    public Task<bool> IsInRoleAsync(ManualNetUserEntity user, Role role)
    {
        return base.IsInRoleAsync(user, role.Name);
    }

    public Task<IdentityResult> CreateAsync(ManualNetUserEntity user, Password password)
    {
        return base.CreateAsync(user, password.ToString());
    }

    public Task<bool> CheckPasswordAsync(ManualNetUserEntity user, Password password)
    {
        return base.CheckPasswordAsync(user, password.ToString());
    }
}
