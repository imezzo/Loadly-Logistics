using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class TripConfiguration : IEntityTypeConfiguration<Trip>
{
    public void Configure(EntityTypeBuilder<Trip> builder)
    {
        builder.ToTable("trips");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(t => t.ContractId)
            .HasColumnName("contract_id")
            .IsRequired(false);
        builder.Property(t => t.ContractVehicleTypeId)
            .HasColumnName("contract_vehicle_type_id")
            .IsRequired(false);
        builder.Property(t => t.ContractSlotId)
            .HasColumnName("contract_slot_id")
            .IsRequired(false);
        builder.Property(t => t.VehicleId)
            .HasColumnName("vehicle_id")
            .IsRequired(false);
        builder.Property(t => t.TailVehicleId)
            .HasColumnName("tail_vehicle_id")
            .IsRequired(false);
        builder.Property(t => t.DriverId)
            .HasColumnName("driver_id")
            .IsRequired(false);
        builder.Property(t => t.ContractorId)
            .HasColumnName("contractor_id")
            .IsRequired(false);
        builder.Property(t => t.InvoiceId)
            .HasColumnName("invoice_id")
            .IsRequired(false);
        builder.Property(t => t.ClaimInvoiceId)
            .HasColumnName("claim_invoice_id")
            .IsRequired(false);
        builder.Property(t => t.ContractorPayoutId)
            .HasColumnName("contractor_payout_id")
            .IsRequired(false);
        builder.Property(t => t.LeadId)
            .HasColumnName("lead_id")
            .IsRequired(false);
        builder.Property(t => t.StartDate)
            .HasColumnName("start_date")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
        builder.Property(t => t.EndDate)
            .HasColumnName("end_date")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
        builder.Property(t => t.FinancialDate)
            .HasColumnName("financial_date")
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(t => t.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(TripStatus.Pending)
            .HasSentinel((TripStatus)0);

        builder.Property(t => t.TripDocumentStatus)
            .HasColumnName("trip_document_status")
            .HasConversion<int>()
            .HasDefaultValue(TripDocumentStatus.Pending)
            .HasSentinel((TripDocumentStatus)0);

        builder.Property(t => t.CustomsStatus)
            .HasColumnName("customs_status")
            .HasConversion<int>()
            .HasDefaultValue(TripCustomsStatus.None)
            .HasSentinel((TripCustomsStatus)0);

        builder.Property(t => t.CustomsTotalAmount)
            .HasColumnName("customs_total_amount")
            .HasPrecision(18, 2)
            .IsRequired(false);

        builder.Property(t => t.DispatcherId)
            .HasColumnName("dispatcher_id")
            .IsRequired(false);
        builder.Property(t => t.DispatcherNotes)
            .HasColumnName("dispatcher_notes")
            .HasColumnType("text")
            .IsRequired(false);
        builder.Property(t => t.ProofOfDelivery)
            .HasColumnName("proof_of_delivery")
            .HasColumnType("text")
            .IsRequired(false);
        builder.Property(t => t.AuditedById)
            .HasColumnName("audited_by_id")
            .IsRequired(false);
        builder.Property(t => t.AuditedAt)
            .HasColumnName("audited_at")
            .IsRequired(false);
        builder.Property(t => t.CompanyUserId)
            .HasColumnName("company_user_id")
            .IsRequired(false);
        builder.Property(t => t.ContractDistanceKm)
            .HasColumnName("contract_distance_km")
            .HasPrecision(18, 2)
            .IsRequired(false);
        builder.Property(t => t.ActualDistanceKm)
            .HasColumnName("actual_distance_km")
            .HasPrecision(18, 2)
            .IsRequired(false);
        builder.Property(t => t.ActualCargoWeightTons)
            .HasColumnName("actual_cargo_weight_tons")
            .HasPrecision(18, 2)
            .IsRequired(false);
        builder.Property(t => t.CargoPrice)
            .HasColumnName("cargo_price")
            .HasPrecision(18, 2)
            .IsRequired(false);
        builder.Property(t => t.SpotPrice)
            .HasColumnName("spot_price")
            .HasPrecision(18, 2)
            .IsRequired(false);
        builder.Property(t => t.ContractorPrice)
            .HasColumnName("contractor_price")
            .HasPrecision(18, 2)
            .IsRequired(false);
        builder.Property(t => t.NumberOfDays)
            .HasColumnName("number_of_days")
            .IsRequired(false);
        builder.Property(t => t.OverNightCostBearer)
            .HasColumnName("over_night_cost_bearer")
            .HasConversion<int>()
            .IsRequired(false);
        builder.Property(t => t.DispatchingMode)
            .HasColumnName("dispatching_mode")
            .HasConversion<int>()
            .IsRequired(false);
        builder.Property(t => t.Type)
            .HasColumnName("type")
            .HasConversion<int>()
            .IsRequired(false);
        builder.Property(t => t.ContactPersonName)
            .HasColumnName("contact_person_name")
            .HasMaxLength(255)
            .IsRequired(false);
        builder.Property(t => t.ContactPersonMobile)
            .HasColumnName("contact_person_mobile")
            .HasMaxLength(20)
            .IsRequired(false);
        builder.Property(t => t.HelpersCount)
            .HasColumnName("helpers_count")
            .IsRequired(false);
        builder.Property(t => t.TotalAddonsCost)
            .HasColumnName("total_addons_cost")
            .HasPrecision(18, 2)
            .IsRequired(false);
        builder.Property(t => t.TotalTripCost)
            .HasColumnName("total_trip_cost")
            .HasPrecision(18, 2)
            .IsRequired(false);
        builder.Property(t => t.IsExternalTrip)
            .HasColumnName("is_external_trip")
            .HasDefaultValue(false)
            .IsRequired();
        builder.Property(t => t.IsBackfill)
            .HasColumnName("is_backfill")
            .IsRequired(false);
        builder.Property(t => t.CollectorId)
            .HasColumnName("collector_id")
            .IsRequired(false);
        builder.Property(t => t.FinanceCollectorId)
            .HasColumnName("finance_collector_id")
            .IsRequired(false);
        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(t => t.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(t => t.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        // Single column indexes
        builder.HasIndex(t => t.ContractId);
        builder.HasIndex(t => t.ContractVehicleTypeId);
        builder.HasIndex(t => t.ContractSlotId);
        builder.HasIndex(t => t.VehicleId);
        builder.HasIndex(t => t.TailVehicleId);
        builder.HasIndex(t => t.DriverId);
        builder.HasIndex(t => t.ContractorId);
        builder.HasIndex(t => t.StartDate);
        builder.HasIndex(t => t.EndDate);
        builder.HasIndex(t => t.FinancialDate);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.TripDocumentStatus);
        builder.HasIndex(t => t.CustomsStatus);
        builder.HasIndex(t => t.InvoiceId);
        builder.HasIndex(t => t.ClaimInvoiceId);
        builder.HasIndex(t => t.ContractorPayoutId);
        builder.HasIndex(t => t.LeadId);
        builder.HasIndex(t => t.DispatcherId);
        builder.HasIndex(t => t.AuditedById);
        builder.HasIndex(t => t.CompanyUserId);
        builder.HasIndex(t => t.CollectorId);
        builder.HasIndex(t => t.FinanceCollectorId);

        // Composite indexes for common query patterns
        builder.HasIndex(t => new { t.Status, t.StartDate });  // Filter by status and date range
        builder.HasIndex(t => new { t.ContractId, t.Status });  // Filter by contract and status
        builder.HasIndex(t => new { t.VehicleId, t.StartDate });  // Filter by vehicle and date range
        builder.HasIndex(t => new { t.DriverId, t.Status });  // Filter by driver and status
        builder.HasIndex(t => new { t.Status, t.CreatedAt });  // Filter by status and creation date

        // Foreign keys
        builder.HasOne(t => t.Contract)
            .WithMany(c => c.Trips)
            .HasForeignKey(t => t.ContractId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.ContractVehicleType)
            .WithMany()
            .HasForeignKey(t => t.ContractVehicleTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.ContractSlot)
            .WithMany()
            .HasForeignKey(t => t.ContractSlotId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Vehicle)
            .WithMany(v => v.Trips)
            .HasForeignKey(t => t.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.TailVehicle)
            .WithMany()
            .HasForeignKey(t => t.TailVehicleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.Driver)
            .WithMany(d => d.Trips)
            .HasForeignKey(t => t.DriverId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.Contractor)
            .WithMany(c => c.Trips)
            .HasForeignKey(t => t.ContractorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(t => t.Recordings)
            .WithOne()
            .HasForeignKey(r => r.RecordableId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(t => t.Waypoints)
            .WithOne()
            .HasForeignKey(w => w.TripId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.TripCharges)
            .WithOne(tc => tc.Trip)
            .HasForeignKey(tc => tc.TripId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Invoice)
           .WithMany(i => i.Trips)
           .HasForeignKey(t => t.InvoiceId)
           .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.ClaimInvoice)
           .WithMany(i => i.ClaimInvoiceTrips)
           .HasForeignKey(t => t.ClaimInvoiceId)
           .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.ContractorPayout)
            .WithMany(p => p.Trips)
            .HasForeignKey(t => t.ContractorPayoutId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Dispatcher)
            .WithMany()
            .HasForeignKey(t => t.DispatcherId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.AuditedBy)
            .WithMany()
            .HasForeignKey(t => t.AuditedById)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.CompanyUser)
            .WithMany()
            .HasForeignKey(t => t.CompanyUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.Collector)
            .WithMany()
            .HasForeignKey(t => t.CollectorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.FinanceCollector)
            .WithMany()
            .HasForeignKey(t => t.FinanceCollectorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.Lead)
            .WithMany(l => l.Trips)
            .HasForeignKey(t => t.LeadId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
