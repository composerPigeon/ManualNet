using Microsoft.EntityFrameworkCore;
using Server.Model.Auth;

namespace Server.Data.EntityContexts;

public static class ApplicationUserContext
{
    public const int NameMaxLength = 64;
    
    public static void MapUserContext(this ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>(user =>
        {
            user.Property(x => x.FirstName)
                .HasMaxLength(NameMaxLength)
                .IsRequired();

            user.Property(x => x.LastName)
                .HasMaxLength(NameMaxLength)
                .IsRequired();
        });
    }
}
