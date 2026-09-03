using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Model.Domain;


namespace Server.Data.EntityContexts;

public class ProductEntityContext : EntityContextBase<ProductEntity>
{
    protected override void MapProperties(EntityTypeBuilder<ProductEntity> entity)
    {
        base.MapProperties(entity);
        
        entity.Property(p => p.Name)
            .HasMaxLength(IEntityContext.MaxNameLength)
            .IsRequired();

        entity.Property(p => p.Description)
            .HasMaxLength(IEntityContext.MaxDescriptionLength)
            .IsRequired();

        entity.HasOne(p => p.Manufacturer)
            .WithMany()
            .HasForeignKey("ManufacturerId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
