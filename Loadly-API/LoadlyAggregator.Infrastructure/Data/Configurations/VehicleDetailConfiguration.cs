using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class VehicleDetailConfiguration : IEntityTypeConfiguration<VehicleDetail>
{
    public void Configure(EntityTypeBuilder<VehicleDetail> builder)
    {
        builder.ToTable("vehicle_details");

        builder.HasKey(vd => vd.Id);

        builder.Property(vd => vd.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(vd => vd.VehicleId)
            .HasColumnName("vehicle_id")
            .IsRequired();

        builder.Property(vd => vd.GovernorateCode)
            .HasColumnName("governorate_code")
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(vd => vd.InsuranceDetails)
            .HasColumnName("insurance_details")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(vd => vd.TemperatureRangeSettng)
            .HasColumnName("temperature_range_setting")
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(vd => vd.CoolingUnitSpecification)
            .HasColumnName("cooling_unit_specification")
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(vd => vd.TemperatureMonotoringCapability)
            .HasColumnName("temperature_monitoring_capability")
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(vd => vd.MaximumLoadHeight)
            .HasColumnName("maximum_load_height")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);

        builder.Property(vd => vd.InternalHeight)
            .HasColumnName("internal_height")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);

        builder.Property(vd => vd.InternalWidth)
            .HasColumnName("internal_width")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);

        builder.Property(vd => vd.InternalLength)
            .HasColumnName("internal_length")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);

        builder.Property(vd => vd.LoadingMechanismType)
            .HasColumnName("loading_mechanism_type")
            .HasConversion<int>()
            .IsRequired(false);



        builder.Property(vd => vd.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.Property(vd => vd.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(vd => vd.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Relationships
        builder.HasOne(vd => vd.Vehicle)
            .WithOne(v => v.VehicleDetail)
            .HasForeignKey<VehicleDetail>(vd => vd.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(vd => vd.VehicleId);
    }
}
