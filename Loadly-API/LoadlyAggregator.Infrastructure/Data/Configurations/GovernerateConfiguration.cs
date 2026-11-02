using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class GovernerateConfiguration : IEntityTypeConfiguration<Governerate>
{
    public void Configure(EntityTypeBuilder<Governerate> builder)
    {
        builder.ToTable("governerates");
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(g => g.NameEn)
            .HasColumnName("name_en")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(g => g.NameAr)
            .HasColumnName("name_ar")
            .HasMaxLength(255);
        builder.Property(g => g.AddedBy)
            .HasColumnName("added_by");
        builder.Property(g => g.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(g => g.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(g => g.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.HasIndex(g => g.NameEn).IsUnique();
        builder.HasIndex(g => g.NameAr).IsUnique();
    }
}
