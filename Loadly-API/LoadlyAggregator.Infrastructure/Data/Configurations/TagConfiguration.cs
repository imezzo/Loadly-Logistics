using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tags");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(t => t.NameAr)
            .HasColumnName("name_ar")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(t => t.NameEn)
            .HasColumnName("name_en")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(t => t.IsPricing)
            .HasColumnName("is_pricing")
            .IsRequired()
            .HasDefaultValue(false);
        builder.Property(t => t.AddedBy)
            .HasColumnName("added_by");
        builder.Property(t => t.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(t => t.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.HasIndex(t => t.NameAr).IsUnique();
        builder.HasIndex(t => t.NameEn).IsUnique();
        builder.HasIndex(t => t.IsPricing);
    }
}
