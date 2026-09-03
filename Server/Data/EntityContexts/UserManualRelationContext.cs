using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Model.Domain;

namespace Server.Data.EntityContexts;

public class UserManualRelationContext : EntityContextBase<UserManualRelation>
{
    protected override void MapProperties(EntityTypeBuilder<UserManualRelation> entity)
    {
        base.MapProperties(entity);
        
        entity.Property(r => r.LocalFileName)
            .HasMaxLength(IEntityContext.MaxNameLength)
            .IsRequired();

        entity.Property(r => r.Rating);

        entity.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasOne(r => r.Manual)
            .WithMany()
            .HasForeignKey("ManualId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
