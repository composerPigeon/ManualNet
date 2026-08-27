using Microsoft.AspNetCore.Identity;
using Server.Model.Auth;

namespace Server.Data;

public static class InitRolesExtension
{
    public static async Task CreateInitialRolesAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var role in Roles.GetInitialRoles())
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}