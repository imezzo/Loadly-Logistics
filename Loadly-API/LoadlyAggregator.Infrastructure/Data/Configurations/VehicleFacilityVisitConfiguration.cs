using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class VehicleFacilityVisitConfiguration : IEntityTypeConfiguration<VehicleFacilityVisit>
{
    public void Configure(EntityTypeBuilder<VehicleFacilityVisit> builder)
    {
        builder.ToTable("vehicle_facility_visits");

        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(v => v.VehicleId)
            .HasColumnName("vehicle_id")
            .IsRequired();

        builder.Property(v => v.VehicleFacilityId)
            .HasColumnName("vehicle_facility_id")
            .IsRequired();

        builder.Property(v => v.ContractSlotId)
            .HasColumnName("contract_slot_id")
            .IsRequired(false);

        builder.Property(v => v.Event)
            .HasColumnName("event")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(v => v.Context)
            .HasColumnName("context")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(v => v.EventDate)
            .HasColumnName("event_date")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(v => v.DriverId)
            .HasColumnName("driver_id")
            .IsRequired(false);

        builder.Property(v => v.Reason)
            .HasColumnName("reason")
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(v => v.AddedBy)
            .HasColumnName("added_by")
            .IsRequired(false);

        builder.Property(v => v.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(v => v.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(v => v.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(v => new { v.VehicleId, v.EventDate });
        builder.HasIndex(v => new { v.VehicleFacilityId, v.EventDate });
        builder.HasIndex(v => v.ContractSlotId);
        builder.HasIndex(v => v.AddedBy);

        builder.HasOne(v => v.Vehicle)
            .WithMany(vehicle => vehicle.VehicleFacilityVisits)
            .HasForeignKey(v => v.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.VehicleFacility)
            .WithMany(location => location.VehicleFacilityVisits)
            .HasForeignKey(v => v.VehicleFacilityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.ContractSlot)
            .WithMany()
            .HasForeignKey(v => v.ContractSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.Driver)
            .WithMany()
            .HasForeignKey(v => v.DriverId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(v => v.AddedByAdmin)
            .WithMany()
            .HasForeignKey(v => v.AddedBy)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
