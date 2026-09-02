using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Model.Auth;
using Server.Model.Domain;
using Shared.Model.Domain;

namespace Server.Data.EntityContexts;

public sealed class ManualEntityContext : EntityContextBase<ManualEntity, Guid>
{
    protected override void MapProperties(EntityTypeBuilder<ManualEntity> entity)
    {
        base.MapProperties(entity);
        
        entity.HasKey(m => m.Id);

        entity.Property(m => m.Id)
            .IsRequired();

        entity.Property(m => m.AddedAt)
            .IsRequired();

        entity.Property(m => m.Language)
            .IsRequired();

        entity.HasOne(m => m.Product)
            .WithMany()
            .HasForeignKey("ProductId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
