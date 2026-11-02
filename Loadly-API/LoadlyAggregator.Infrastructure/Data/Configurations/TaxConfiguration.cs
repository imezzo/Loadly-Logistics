using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class TaxConfiguration : IEntityTypeConfiguration<Tax>
{
    public void Configure(EntityTypeBuilder<Tax> builder)
    {
        builder.ToTable("taxes");
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
        builder.Property(t => t.Type)
            .HasColumnName("type")
            .HasConversion<int>()
            .HasDefaultValue(TaxType.Fixed)
            .HasSentinel((TaxType)0);
        builder.Property(t => t.Value)
            .HasColumnName("value")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Property(t => t.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(TaxStatus.ACTIVE)
            .HasSentinel(TaxStatus.ACTIVE);
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
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.Type);
    }
}
