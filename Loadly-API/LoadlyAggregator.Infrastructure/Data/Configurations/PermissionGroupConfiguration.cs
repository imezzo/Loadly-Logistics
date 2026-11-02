using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class PermissionGroupConfiguration : IEntityTypeConfiguration<PermissionGroup>
{
    public void Configure(EntityTypeBuilder<PermissionGroup> builder)
    {
        builder.ToTable("permission_groups");
        builder.HasKey(pg => pg.Id);
        builder.Property(pg => pg.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");
        builder.Property(pg => pg.Name)
            .HasColumnName("name")
            .HasMaxLength(255);
        builder.Property(pg => pg.NameAr)
            .HasColumnName("name_ar")
            .HasMaxLength(255);
        builder.Property(pg => pg.Status)
            .HasColumnName("status")
            .HasConversion<int>();
        builder.Property(pg => pg.Type)
            .HasColumnName("type")
            .HasMaxLength(100)
            .HasDefaultValue("company");
        builder.Property(pg => pg.DeletedAt)
            .HasColumnName("deleted_at");
        builder.Property(pg => pg.CreatedAt)
            .HasColumnName("created_at");
        builder.Property(pg => pg.UpdatedAt)
            .HasColumnName("updated_at");
        builder.HasIndex(pg => pg.Type);
    }
}
