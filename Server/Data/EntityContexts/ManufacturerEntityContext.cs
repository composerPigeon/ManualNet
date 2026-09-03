using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Model.Domain;

namespace Server.Data.EntityContexts;

public sealed class ManufacturerEntityContext : EntityContextBase<ManufacturerEntity>
{
    protected override void MapProperties(EntityTypeBuilder<ManufacturerEntity> entity)
    {
        base.MapProperties(entity);
        
        entity.Property(e => e.Name)
            .HasMaxLength(IEntityContext.MaxNameLength)
            .IsRequired();
            
        entity.Property(e => e.Description)
            .HasMaxLength(IEntityContext.MaxDescriptionLength)
            .IsRequired();
    }
}
