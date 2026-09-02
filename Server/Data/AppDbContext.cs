using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Server.Data.EntityContexts;
using Server.Data.Managers;
using Server.Model;
using Server.Model.Auth;
using Server.Model.Domain;

namespace Server.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ManualNetUserEntity>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyEntityContexts();
    }
}

public static class AppDbContextModelBuilderExtensions
{
    public static void ApplyEntityContexts(this ModelBuilder builder)
    {
        foreach (var context in EntityContextCollection.Default.GetAllContexts())
        {
            context.MapBuilder(builder);
        }
    }
}
