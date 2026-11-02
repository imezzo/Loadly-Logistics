using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ColorConfiguration : IEntityTypeConfiguration<Color>
{
    public void Configure(EntityTypeBuilder<Color> builder)
    {
        builder.ToTable("colors");
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
        builder.Property(c => c.ColorHash)
            .HasColumnName("color_hash")
            .IsRequired()
            .HasMaxLength(7);
        builder.Property(c => c.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(UserStatus.ACTIVE)
            .HasSentinel((UserStatus)0);
        builder.Property(c => c.AddedBy)
            .HasColumnName("added_by");
        builder.Property(c => c.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.NameAr).IsUnique();
        builder.HasIndex(c => c.NameEn).IsUnique();
        builder.HasIndex(c => c.ColorHash).IsUnique();
    }
}
