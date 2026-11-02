using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("vehicles", table =>
        {
            table.HasCheckConstraint("CK_Vehicle_NotSelfHead", "\"head_vehicle_id\" IS NULL OR \"head_vehicle_id\" != id");
        });

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(v => v.ContractId)
            .HasColumnName("contract_id")
            .IsRequired(false);

        builder.Property(v => v.ContractorId)
            .HasColumnName("contractor_id")
            .IsRequired(false);

        builder.Property(v => v.SectorId)
            .HasColumnName("sector_id")
            .IsRequired(false);

        builder.Property(v => v.HeadVehicleId)
            .HasColumnName("head_vehicle_id")
            .IsRequired(false);

        builder.Property(v => v.PlateNo)
            .HasColumnName("plate_no")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.PlateLetters)
            .HasColumnName("plate_letters")
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(vd => vd.AddedBy)
            .HasColumnName("added_by");

        builder.Property(vd => vd.VehicleTypeSettings)
           .HasColumnName("vehicle_type_settings")
           .HasMaxLength(1000)
           .IsRequired(false);

        builder.Property(v => v.MakeYear)
            .HasColumnName("make_year");

        builder.Property(v => v.InsuranceId)
            .HasColumnName("insurance_id");

        builder.Property(v => v.PurchaseDate)
            .HasColumnName("purchase_date")
            .IsRequired(false);

        builder.Property(v => v.InitialOdometerReading)
            .HasColumnName("initial_odometer_reading")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);

        builder.Property(v => v.VinNo)
            .HasColumnName("vin_no")
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(v => v.ColorId)
            .HasColumnName("color_id");

        builder.Property(v => v.CapacityUnitId)
            .HasColumnName("capacity_unit_id");

        builder.Property(v => v.DriverId)
            .HasColumnName("driver_id");

        builder.Property(v => v.CurrentServiceFacilityId)
            .HasColumnName("current_service_facility_id")
            .IsRequired(false);

        builder.Property(v => v.AssignedParkingFacilityId)
            .HasColumnName("assigned_parking_facility_id")
            .IsRequired(false);

        builder.Property(v => v.VehicleTypeId)
            .HasColumnName("vehicle_type_id")
            .IsRequired();

        builder.Property(v => v.BrandModelID)
            .HasColumnName("brand_model_id")
            .IsRequired(false);

        builder.Property(v => v.TankCapacity)
            .HasColumnName("tank_capacity")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);

        builder.Property(v => v.Capacity)
            .HasColumnName("capacity")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);

        builder.Property(v => v.FuelTypeId)
            .HasColumnName("fuel_type_id")
            .IsRequired(false);

        builder.Property(v => v.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(VehicleStatus.Available)
            .HasSentinel((VehicleStatus)0);

        builder.Property(v => v.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.Property(v => v.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(v => v.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Relationships
        builder.HasOne(v => v.Insurance)
            .WithMany()
            .HasForeignKey(v => v.InsuranceId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(v => v.Color)
            .WithMany()
            .HasForeignKey(v => v.ColorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(v => v.CapacityUnit)
            .WithMany()
            .HasForeignKey(v => v.CapacityUnitId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(v => v.Driver)
            .WithMany(d => d.Vehicles)
            .HasForeignKey(v => v.DriverId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(v => v.CurrentServiceFacility)
            .WithMany()
            .HasForeignKey(v => v.CurrentServiceFacilityId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(v => v.AssignedParkingFacility)
            .WithMany()
            .HasForeignKey(v => v.AssignedParkingFacilityId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(v => v.VehicleType)
            .WithMany()
            .HasForeignKey(v => v.VehicleTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.VehicleBrandModel)
            .WithMany()
            .HasForeignKey(v => v.BrandModelID)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(v => v.Contract)
            .WithMany(c => c.Vehicles)
            .HasForeignKey(v => v.ContractId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(v => v.Contractor)
            .WithMany(c => c.Vehicles)
            .HasForeignKey(v => v.ContractorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(v => v.Sector)
            .WithMany()
            .HasForeignKey(v => v.SectorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(v => v.HeadVehicle)
            .WithMany(v => v.TailVehicles)
            .HasForeignKey(v => v.HeadVehicleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(v => v.FuelType)
            .WithMany()
            .HasForeignKey(v => v.FuelTypeId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(v => v.Status);
        builder.HasIndex(v => new { v.PlateNo, v.PlateLetters })
            .IsUnique();
        builder.HasIndex(v => v.ColorId);
        builder.HasIndex(v => v.CapacityUnitId);
        builder.HasIndex(v => v.VehicleTypeId);
        builder.HasIndex(v => v.BrandModelID);

        builder.HasIndex(v => v.DriverId);
        builder.HasIndex(v => v.CurrentServiceFacilityId);
        builder.HasIndex(v => v.AssignedParkingFacilityId);
        builder.HasIndex(v => v.ContractId);
        builder.HasIndex(v => v.ContractorId);
        builder.HasIndex(v => v.SectorId);
        builder.HasIndex(v => v.HeadVehicleId);
        builder.HasIndex(v => new { v.HeadVehicleId, v.VehicleTypeId })
            .HasDatabaseName("IX_vehicles_head_vehicle_id_vehicle_type_id_tail_lookup")
            .HasFilter("head_vehicle_id IS NOT NULL");
        builder.HasIndex(v => v.FuelTypeId);
    }
}
