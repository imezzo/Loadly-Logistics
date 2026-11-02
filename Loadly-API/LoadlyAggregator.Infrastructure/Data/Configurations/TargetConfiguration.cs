using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class TargetConfiguration : IEntityTypeConfiguration<Target>
{
    public void Configure(EntityTypeBuilder<Target> builder)
    {
        builder.ToTable("targets");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(t => t.Month)
            .HasColumnName("month")
            .HasConversion<int>()
            .IsRequired();
        builder.Property(t => t.Year)
            .HasColumnName("year")
            .IsRequired();
        builder.Property(t => t.TargetType)
            .HasColumnName("target_type")
            .HasConversion<int>()
            .IsRequired();
        builder.Property(t => t.TargetValue)
            .HasColumnName("target_value")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(t => t.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(t => t.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(t => new { t.Month, t.Year, t.TargetType })
            .IsUnique()
            .HasDatabaseName("ix_targets_month_year_target_type_unique")
            .HasFilter("\"deleted_at\" IS NULL");
    }
}
