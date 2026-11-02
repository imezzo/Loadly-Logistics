using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ContainerConfiguration : IEntityTypeConfiguration<Container>
{
    public void Configure(EntityTypeBuilder<Container> builder)
    {
        builder.ToTable("containers");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(c => c.NameAr)
            .HasColumnName("name_ar")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(c => c.NameEn)
            .HasColumnName("name_en")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(c => c.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(ContainerStatus.ACTIVE)
            .HasSentinel(ContainerStatus.ACTIVE);
        builder.Property(c => c.AddedBy)
            .HasColumnName("added_by");
        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(c => c.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(c => c.NameAr)
            .IsUnique()
            .HasDatabaseName("ix_containers_name_ar_unique")
            .HasFilter("\"deleted_at\" IS NULL");

        builder.HasIndex(c => c.NameEn)
            .IsUnique()
            .HasDatabaseName("ix_containers_name_en_unique")
            .HasFilter("\"deleted_at\" IS NULL");

        builder.HasIndex(c => c.Status);
    }
}
