using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");
        builder.Property(p => p.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(p => p.NameAr)
            .HasColumnName("name_ar")
            .HasMaxLength(255);
        builder.Property(p => p.GuardName)
            .HasColumnName("guard_name")
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(p => p.Status)
            .HasColumnName("status")
            .HasConversion<int>();
        builder.Property(p => p.PermissionGroupId)
            .HasColumnName("permission_group_id")
            .HasColumnType("bigint");
        builder.Property(p => p.DeletedAt)
            .HasColumnName("deleted_at");
        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at");
        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at");
        builder.HasIndex(p => new { p.Name, p.GuardName }).IsUnique();
        builder.HasOne(p => p.PermissionGroup)
            .WithMany(pg => pg.Permissions)
            .HasForeignKey(p => p.PermissionGroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
