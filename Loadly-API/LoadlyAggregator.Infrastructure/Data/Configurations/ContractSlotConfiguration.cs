using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ContractSlotConfiguration : IEntityTypeConfiguration<ContractSlot>
{
    public void Configure(EntityTypeBuilder<ContractSlot> builder)
    {
        builder.ToTable("contract_slots");
        builder.HasKey(cs => cs.Id);
        builder.Property(cs => cs.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(cs => cs.AssignedAt)
            .HasColumnName("assigned_at")
            .IsRequired();
        builder.Property(cs => cs.ArchivedAt)
            .HasColumnName("archived_at")
            .IsRequired(false);
        builder.Property(cs => cs.ContractId)
            .HasColumnName("contract_id")
            .IsRequired();
        builder.Property(cs => cs.VehicleTypeId)
            .HasColumnName("vehicle_type_id")
            .IsRequired();
        builder.Property(cs => cs.VehicleId)
            .HasColumnName("vehicle_id")
            .IsRequired(false);
        builder.Property(cs => cs.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired()
            .HasDefaultValue(ContractSlotStatus.ACTIVE);
        builder.Property(cs => cs.ConsumptionRate)
            .HasColumnName("consumption_rate")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(cs => cs.TripsPerMonth)
            .HasColumnName("trips_per_month")
            .IsRequired(false);
        builder.Property(cs => cs.TripsPerDay)
            .HasColumnName("trips_per_day")
            .IsRequired(false);
        builder.Property(cs => cs.MaxKms)
            .HasColumnName("max_kms")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(cs => cs.MonthlyRental)
            .HasColumnName("monthly_rental")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(cs => cs.DailyValue)
            .HasColumnName("daily_value")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(cs => cs.OverConsumption)
            .HasColumnName("over_consumption")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(cs => cs.MinBillableWeightTons)
            .HasColumnName("min_billable_weight_tons")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(cs => cs.MaxPayloadTons)
            .HasColumnName("max_payload_tons")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(cs => cs.OverWeightPricePerTon)
            .HasColumnName("overweight_price_per_ton")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(cs => cs.ContractorOverWeightPricePerTon)
            .HasColumnName("contractor_overweight_price_per_ton")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(cs => cs.ExtraWorkingDayFees)
            .HasColumnName("extra_working_day_fees")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(cs => cs.AddedBy)
            .HasColumnName("added_by")
            .IsRequired(false);
        builder.Property(cs => cs.ReplacedSlotId)
            .HasColumnName("replaced_slot_id")
            .IsRequired(false);
        builder.Property(cs => cs.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(cs => cs.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(cs => cs.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(cs => cs.ContractId);
        builder.HasIndex(cs => cs.VehicleTypeId);
        builder.HasIndex(cs => cs.VehicleId);
        builder.HasIndex(cs => cs.VehicleId)
            .IsUnique()
            .HasDatabaseName("ix_contract_slots_active_vehicle_id")
            .HasFilter("\"deleted_at\" IS NULL AND \"vehicle_id\" IS NOT NULL AND \"status\" = 1");

        builder.HasIndex(cs => cs.ReplacedSlotId);

        builder.HasOne(cs => cs.Contract)
            .WithMany(c => c.ContractSlots)
            .HasForeignKey(cs => cs.ContractId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(cs => cs.VehicleType)
            .WithMany()
            .HasForeignKey(cs => cs.VehicleTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(cs => cs.Vehicle)
            .WithMany()
            .HasForeignKey(cs => cs.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(cs => cs.ReplacedSlot)
            .WithMany()
            .HasForeignKey(cs => cs.ReplacedSlotId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
