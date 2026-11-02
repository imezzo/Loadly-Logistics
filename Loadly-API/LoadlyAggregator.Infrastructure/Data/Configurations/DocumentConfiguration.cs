using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("documents");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(d => d.EntityId)
            .HasColumnName("entity_id")
            .HasConversion<int>()
            .IsRequired()
            .HasDefaultValue(DocumentEntityType.ADMIN)
            .HasSentinel(DocumentEntityType.ADMIN);
        builder.Property(d => d.NameEn)
            .HasColumnName("name_en")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(d => d.NameAr)
            .HasColumnName("name_ar")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(d => d.Required)
            .HasColumnName("required")
            .HasDefaultValue(false);
        builder.Property(d => d.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(DocumentStatus.ACTIVE)
            .HasSentinel(DocumentStatus.INACTIVE);
        builder.Property(d => d.AddedBy)
            .HasColumnName("added_by");
        builder.Property(d => d.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(d => d.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.HasIndex(d => d.EntityId);
        builder.HasIndex(d => d.Status);
        builder.HasIndex(d => new { d.EntityId, d.NameEn }).IsUnique();
        builder.HasIndex(d => new { d.EntityId, d.NameAr }).IsUnique();
    }
}
