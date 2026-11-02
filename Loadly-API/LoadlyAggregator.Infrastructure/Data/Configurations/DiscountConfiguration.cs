using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.ToTable("discounts");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(d => d.NameAr)
            .HasColumnName("name_ar")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(d => d.NameEn)
            .HasColumnName("name_en")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(d => d.Type)
            .HasColumnName("type")
            .HasConversion<int>()
            .HasDefaultValue(DiscountType.Fixed)
            .HasSentinel((DiscountType)0);
        builder.Property(d => d.Value)
            .HasColumnName("value")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Property(d => d.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(DiscountStatus.ACTIVE)
            .HasSentinel(DiscountStatus.ACTIVE);
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
        builder.HasIndex(d => d.Status);
        builder.HasIndex(d => d.Type);
    }
}
