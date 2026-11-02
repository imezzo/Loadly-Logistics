using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class VehicleTypeConfiguration : IEntityTypeConfiguration<VehicleType>
{
    public void Configure(EntityTypeBuilder<VehicleType> builder)
    {
        builder.ToTable("vehicle_types");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(v => v.NameAr)
            .HasColumnName("name_ar")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(v => v.NameEn)
            .HasColumnName("name_en")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(v => v.Category)
            .HasColumnName("category")
            .HasConversion<int>()
            .HasDefaultValue(VehicleTypeCategory.Fridge)
            .HasSentinel((VehicleTypeCategory)0);
        builder.Property(v => v.VehicleUnitType)
            .HasColumnName("vehicle_unit_type")
            .HasConversion<int?>()
            .IsRequired(false);
        builder.Property(v => v.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(VehicleTypeStatus.ACTIVE)
            .HasSentinel((VehicleTypeStatus)0);
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
        builder.HasIndex(v => v.Category);
    }
}
