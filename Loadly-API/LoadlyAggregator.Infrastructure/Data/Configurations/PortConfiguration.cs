using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class PortConfiguration : IEntityTypeConfiguration<Port>
{
    public void Configure(EntityTypeBuilder<Port> builder)
    {
        builder.ToTable("ports");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(p => p.NameAr)
            .HasColumnName("name_ar")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(p => p.NameEn)
            .HasColumnName("name_en")
            .HasMaxLength(255);
        builder.Property(p => p.Country)
            .HasColumnName("country")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(p => p.PortCode)
            .HasColumnName("port_code")
            .HasMaxLength(50);
        builder.Property(p => p.Type)
            .HasColumnName("type")
            .HasConversion<int>()
            .IsRequired();
        builder.Property(p => p.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(PortStatus.ACTIVE)
            .HasSentinel(PortStatus.ACTIVE);
        builder.Property(p => p.AddedBy)
            .HasColumnName("added_by");
        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(p => p.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(p => new { p.NameAr, p.Country, p.Type })
            .IsUnique()
            .HasDatabaseName("ix_ports_name_ar_country_type_unique")
            .HasFilter("\"deleted_at\" IS NULL");

        builder.HasIndex(p => p.PortCode)
            .IsUnique()
            .HasDatabaseName("ix_ports_port_code_unique")
            .HasFilter("\"deleted_at\" IS NULL AND \"port_code\" IS NOT NULL");

        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.Type);
    }
}
