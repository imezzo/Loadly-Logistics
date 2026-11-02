using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("invoices");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(i => i.InvoiceNumber)
            .HasColumnName("invoice_number")
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(i => i.Sequence)
            .HasColumnName("sequence")
            .IsRequired(false);
        builder.Property(i => i.InvoiceType)
            .HasColumnName("invoice_type")
            .HasConversion<int>()
            .HasDefaultValue(InvoiceType.Trips)
            .HasSentinel((InvoiceType)0);
        builder.Property(i => i.ContractId)
            .HasColumnName("contract_id")
            .IsRequired(false);
        builder.Property(i => i.LeadId)
            .HasColumnName("lead_id")
            .IsRequired(false);
        builder.Property(i => i.StartDate)
            .HasColumnName("start_date")
            .HasColumnType("date")
            .IsRequired();
        builder.Property(i => i.EndDate)
            .HasColumnName("end_date")
            .HasColumnType("date")
            .IsRequired();
        builder.Property(i => i.IssueDate)
            .HasColumnName("issue_date")
            .HasColumnType("date")
            .IsRequired(false);
        builder.Property(i => i.DueDate)
            .HasColumnName("due_date")
            .HasColumnType("date")
            .IsRequired(false);
        builder.Property(i => i.SubTotal)
            .HasColumnName("sub_total")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Property(i => i.SlotChargesSubTotal)
            .HasColumnName("slot_charges_sub_total")
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0m)
            .IsRequired();
        builder.Property(i => i.ContractBillingSnapshotId)
            .HasColumnName("contract_billing_snapshot_id")
            .IsRequired(false);
        builder.Property(i => i.SlotChargesCalculatedAt)
            .HasColumnName("slot_charges_calculated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
        builder.Property(i => i.SlotWorkingDaysAmount)
            .HasColumnName("slot_working_days_amount")
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0m);
        builder.Property(i => i.SlotExtraWorkingDaysAmount)
            .HasColumnName("slot_extra_working_days_amount")
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0m);
        builder.Property(i => i.SlotExtraKmsAmount)
            .HasColumnName("slot_extra_kms_amount")
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0m);
        builder.Property(i => i.SlotExtraTripsAmount)
            .HasColumnName("slot_extra_trips_amount")
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0m);
        builder.Property(i => i.SlotWorkingDaysCount)
            .HasColumnName("slot_working_days_count")
            .HasDefaultValue(0);
        builder.Property(i => i.SlotExtraWorkingDaysCount)
            .HasColumnName("slot_extra_working_days_count")
            .HasDefaultValue(0);
        builder.Property(i => i.SlotExtraTripsCount)
            .HasColumnName("slot_extra_trips_count")
            .HasDefaultValue(0);
        builder.Property(i => i.SlotExtraKmsCount)
            .HasColumnName("slot_extra_kms_count")
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0m);
        builder.Property(i => i.TripsSubTotal)
            .HasColumnName("trips_sub_total")
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0m)
            .IsRequired();
        builder.Property(i => i.TripChargesCalculatedAt)
            .HasColumnName("trip_charges_calculated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
        builder.Property(i => i.TaxId)
            .HasColumnName("tax_id")
            .IsRequired(false);
        builder.Property(i => i.DiscountId)
            .HasColumnName("discount_id")
            .IsRequired(false);
        builder.Property(i => i.TaxType)
            .HasColumnName("tax_type")
            .HasConversion<int>()
            .IsRequired(false);
        builder.Property(i => i.TaxValue)
            .HasColumnName("tax_value")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(i => i.TaxAmount)
            .HasColumnName("tax_amount")
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0m)
            .IsRequired();
        builder.Property(i => i.DiscountType)
            .HasColumnName("discount_type")
            .HasConversion<int>()
            .IsRequired(false);
        builder.Property(i => i.DiscountValue)
            .HasColumnName("discount_value")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(i => i.DiscountAmount)
            .HasColumnName("discount_amount")
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0m)
            .IsRequired();
        builder.Property(i => i.TotalAmount)
            .HasColumnName("total_amount")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Property(i => i.TaxedInvoiceAttachment)
            .HasColumnName("taxed_invoice_attachment")
            .HasMaxLength(500)
            .IsRequired(false);
        builder.Property(i => i.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(InvoiceStatus.UnPaid)
            .HasSentinel((InvoiceStatus)0);
        builder.Property(i => i.CancelledAt)
            .HasColumnName("cancelled_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
        builder.Property(i => i.CancellationReason)
            .HasColumnName("cancellation_reason")
            .HasMaxLength(500)
            .IsRequired(false);
        builder.Property(i => i.AddedBy)
            .HasColumnName("added_by")
            .IsRequired(false);
        builder.Property(i => i.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(i => i.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(i => i.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(i => i.InvoiceNumber)
            .IsUnique()
            .HasFilter("\"deleted_at\" IS NULL");

        builder.HasIndex(i => i.ContractId);
        builder.HasIndex(i => i.LeadId);
        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.InvoiceType);
        builder.HasIndex(i => i.StartDate);
        builder.HasIndex(i => i.EndDate);
        builder.HasIndex(i => new { i.ContractId, i.Status });

        builder.HasOne(i => i.ContractBillingSnapshot)
            .WithMany()
            .HasForeignKey(i => i.ContractBillingSnapshotId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne(i => i.Contract)
            .WithMany()
            .HasForeignKey(i => i.ContractId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(i => i.Lead)
            .WithMany()
            .HasForeignKey(i => i.LeadId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(i => i.Tax)
            .WithMany()
            .HasForeignKey(i => i.TaxId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(i => i.Discount)
            .WithMany()
            .HasForeignKey(i => i.DiscountId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
