using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Server.Data.Managers;
using Server.Model.Auth;
using Server.Options;
using Shared.Model.Auth;
using Shared.Requests;

namespace Server.Data;

public static class InitAdminUserExtension
{
    public static async Task CreateInitialAdminAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;
        var options = services.GetRequiredService<IOptions<AdminUserOptions>>().Value;
        var userManager = services.GetRequiredService<IManualNetUserManager>();

        ManualNetEmail.TryParseFrom(options.Email, out var email);
        var password = Password.Parse(options.Password);
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            var request = new RegisterRequest(email, password, options.FirstName, options.LastName);
            user = ManualNetUserEntity.From(request);

            var createResult = await userManager.CreateAsync(user, password);
            EnsureSucceeded(createResult, "create the initial administrator account");
        }

        if (!await userManager.IsInRoleAsync(user, Role.Admin))
        {
            var roleResult = await userManager.AddToRoleAsync(user, Role.Admin);
            EnsureSucceeded(roleResult, "assign the administrator role");
        }
    }

    private static void EnsureSucceeded(IdentityResult result, string operation)
    {
        if (result.Succeeded)
            return;

        var errors = string.Join("; ", result.Errors.Select(error => $"{error.Code}: {error.Description}"));
        throw new InvalidOperationException($"Failed to {operation}. {errors}");
    }
}
