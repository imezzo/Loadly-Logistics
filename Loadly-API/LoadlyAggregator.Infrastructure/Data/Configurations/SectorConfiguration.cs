using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class SectorConfiguration : IEntityTypeConfiguration<Sector>
{
    public void Configure(EntityTypeBuilder<Sector> builder)
    {
        builder.ToTable("sectors");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(s => s.NameEn)
            .HasColumnName("name_en")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(s => s.NameAr)
            .HasColumnName("name_ar")
            .HasMaxLength(255);
        builder.Property(s => s.AddedBy)
            .HasColumnName("added_by");
        builder.Property(s => s.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(s => s.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.HasIndex(s => s.NameEn).IsUnique();
        builder.HasIndex(s => s.NameAr).IsUnique();
    }
}
