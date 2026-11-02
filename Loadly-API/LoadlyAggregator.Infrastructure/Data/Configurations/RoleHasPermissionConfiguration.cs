using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class RoleHasPermissionConfiguration : IEntityTypeConfiguration<RoleHasPermission>
{
    public void Configure(EntityTypeBuilder<RoleHasPermission> builder)
    {
        builder.ToTable("role_has_permissions");
        builder.HasKey(rhp => new { rhp.PermissionId, rhp.RoleId });
        builder.Property(rhp => rhp.PermissionId)
            .HasColumnName("permission_id")
            .HasColumnType("bigint");
        builder.Property(rhp => rhp.RoleId)
            .HasColumnName("role_id")
            .HasColumnType("bigint");
        builder.HasOne(rhp => rhp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rhp => rhp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(rhp => rhp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rhp => rhp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(rhp => rhp.RoleId);


        builder.HasQueryFilter(rhp => rhp.Permission.DeletedAt == null && rhp.Role.DeletedAt == null);
    }
}
