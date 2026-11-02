using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ContractVehicleTypeConfiguration : IEntityTypeConfiguration<ContractVehicleType>
{
    public void Configure(EntityTypeBuilder<ContractVehicleType> builder)
    {
        builder.ToTable("contract_vehicle_types");
        builder.HasKey(cv => cv.Id);
        builder.Property(cv => cv.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(cv => cv.ContractId)
            .HasColumnName("contract_id")
            .IsRequired();
        builder.Property(cv => cv.VehicleTypeId)
            .HasColumnName("vehicle_type_id")
            .IsRequired();
        builder.Property(cv => cv.ConsumptionRate)
            .HasColumnName("consumption_rate")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(cv => cv.TripsPerMonth)
            .HasColumnName("trips_per_month")
            .IsRequired(false);
        builder.Property(cv => cv.TripsPerDay)
            .HasColumnName("trips_per_day")
            .IsRequired(false);
        builder.Property(cv => cv.MaxKms)
            .HasColumnName("max_kms")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(cv => cv.MonthlyRental)
            .HasColumnName("monthly_rental")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(cv => cv.DailyValue)
            .HasColumnName("daily_value")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(cv => cv.OverConsumption)
            .HasColumnName("over_consumption")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(cv => cv.Quantity)
            .HasColumnName("quantity")
            .HasDefaultValue(1)
            .IsRequired();
        builder.Property(cv => cv.MinBillableWeightTons)
            .HasColumnName("min_billable_weight_tons")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(cv => cv.MaxPayloadTons)
            .HasColumnName("max_payload_tons")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(cv => cv.OverWeightPricePerTon)
            .HasColumnName("overweight_price_per_ton")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(cv => cv.ContractorOverWeightPricePerTon)
            .HasColumnName("contractor_overweight_price_per_ton")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(cv => cv.ExtraWorkingDayFees)
            .HasColumnName("extra_working_day_fees")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(cv => cv.AddedBy)
            .HasColumnName("added_by")
            .IsRequired(false);
        builder.Property(cv => cv.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(cv => cv.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(cv => cv.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(cv => cv.ContractId);
        builder.HasIndex(cv => cv.VehicleTypeId);

        builder.HasOne(cv => cv.Contract)
            .WithMany(c => c.ContractVehicleTypes)
            .HasForeignKey(cv => cv.ContractId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(cv => cv.VehicleType)
            .WithMany()
            .HasForeignKey(cv => cv.VehicleTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
