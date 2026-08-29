using Microsoft.EntityFrameworkCore;
using Server.Model.Auth;

namespace Server.Data.EntityContexts;

public static class RefreshTokenEntityContext
{
    public const int TokenHashMaxLength = 64;
    public const int UserIdMaxLength = 450;
    
    public static void MapRefreshTokenContext(this ModelBuilder builder)
    {
        builder.Entity<RefreshTokenEntity>(refreshToken =>
        {
            refreshToken.HasKey(x => x.Id);

            refreshToken.Property(x => x.TokenHash)
                .HasMaxLength(TokenHashMaxLength)
                .IsRequired();

            refreshToken.Property<string>("UserId")
                .HasMaxLength(UserIdMaxLength)
                .IsRequired();

            refreshToken.HasIndex(x => x.TokenHash)
                .IsUnique();

            refreshToken.HasOne(x => x.UserEntity)
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
