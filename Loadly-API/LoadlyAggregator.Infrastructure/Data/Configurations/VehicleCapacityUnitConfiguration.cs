using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class VehicleCapacityUnitConfiguration : IEntityTypeConfiguration<VehicleCapacityUnit>
{
    public void Configure(EntityTypeBuilder<VehicleCapacityUnit> builder)
    {
        builder.ToTable("vehicles_capacity_units");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(v => v.CapacityTypeEn)
            .HasColumnName("capacity_type_en")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(v => v.CapacityTypeAr)
            .HasColumnName("capacity_type_ar")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(v => v.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(UserStatus.ACTIVE)
            .HasSentinel((UserStatus)0);
        builder.Property(v => v.AddedBy)
            .HasColumnName("added_by");
        builder.Property(v => v.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(v => v.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(v => v.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.HasIndex(v => v.Status);
    }
}
