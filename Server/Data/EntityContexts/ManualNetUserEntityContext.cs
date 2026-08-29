using Microsoft.EntityFrameworkCore;
using Server.Model.Auth;

namespace Server.Data.EntityContexts;

public static class ManualNetUserEntityContext
{
    public const int NameMaxLength = 64;
    public const int EmailMaxLength = 256;
    
    public static void MapUserContext(this ModelBuilder builder)
    {
        builder.Entity<ManualNetUserEntity>(user =>
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
