using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Model.Auth;

namespace Server.Data.EntityContexts;

public sealed class ManualNetUserEntityContext : EntityContextBase<ManualNetUserEntity, string>
{

    protected override void MapProperties(EntityTypeBuilder<ManualNetUserEntity> entity)
    {
        base.MapProperties(entity);

        entity.Property(x => x.FirstName)
            .HasMaxLength(IEntityContext.MaxNameLength)
            .IsRequired();

        entity.Property(x => x.LastName)
            .HasMaxLength(IEntityContext.MaxNameLength)
            .IsRequired();
    }
}
