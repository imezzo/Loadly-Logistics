using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ModelHasPermissionConfiguration : IEntityTypeConfiguration<ModelHasPermission>
{
    public void Configure(EntityTypeBuilder<ModelHasPermission> builder)
    {
        builder.ToTable("model_has_permissions");
        builder.HasKey(mhp => new { mhp.PermissionId, mhp.ModelId, mhp.ModelType });
        builder.Property(mhp => mhp.PermissionId)
            .HasColumnName("permission_id")
            .HasColumnType("bigint");
        builder.Property(mhp => mhp.ModelId)
            .HasColumnName("model_id")
            .HasColumnType("bigint");
        builder.Property(mhp => mhp.ModelType)
            .HasColumnName("model_type")
            .IsRequired()
            .HasMaxLength(255);
        builder.HasOne(mhp => mhp.Permission)
            .WithMany(p => p.ModelPermissions)
            .HasForeignKey(mhp => mhp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(mhp => new { mhp.ModelId, mhp.ModelType });


        builder.HasQueryFilter(mhp => mhp.Permission.DeletedAt == null);
    }
}
