using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ShippingLineConfiguration : IEntityTypeConfiguration<ShippingLine>
{
    public void Configure(EntityTypeBuilder<ShippingLine> builder)
    {
        builder.ToTable("shipping_lines");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(s => s.NameAr)
            .HasColumnName("name_ar")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(s => s.NameEn)
            .HasColumnName("name_en")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(s => s.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(ShippingLineStatus.ACTIVE)
            .HasSentinel(ShippingLineStatus.ACTIVE);
        builder.Property(s => s.AddedBy)
            .HasColumnName("added_by");
        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(s => s.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(s => s.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(s => s.NameAr)
            .IsUnique()
            .HasDatabaseName("ix_shipping_lines_name_ar_unique")
            .HasFilter("\"deleted_at\" IS NULL");

        builder.HasIndex(s => s.NameEn)
            .IsUnique()
            .HasDatabaseName("ix_shipping_lines_name_en_unique")
            .HasFilter("\"deleted_at\" IS NULL");

        builder.HasIndex(s => s.Status);
    }
}
