using Microsoft.AspNetCore.Identity;
using Shared.Model.Auth;

namespace Server.Data;

public static class InitRolesExtension
{
    public static async Task CreateInitialRolesAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var role in Roles.GetAll())
        {
            if (!await roleManager.RoleExistsAsync(role.Name))
            {
                await roleManager.CreateAsync(new IdentityRole(role.Name));
            }
        }
    }
}
