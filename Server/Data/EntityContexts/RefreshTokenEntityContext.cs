using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Model.Auth;

namespace Server.Data.EntityContexts;

public sealed class RefreshTokenEntityContext : EntityContextBase<RefreshTokenEntity, Guid>
{
    public const int TokenHashMaxLength = 64;
    public const int UserIdMaxLength = 450;

    protected override void MapProperties(EntityTypeBuilder<RefreshTokenEntity> refreshToken)
    {
        base.MapProperties(refreshToken);

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
    }
}
