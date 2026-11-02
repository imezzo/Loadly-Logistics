using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ContractBillingSnapshotConfiguration : IEntityTypeConfiguration<ContractBillingSnapshot>
{
    public void Configure(EntityTypeBuilder<ContractBillingSnapshot> builder)
    {
        builder.ToTable("contract_billing_snapshots");
        builder.HasKey(snapshot => snapshot.Id);
        builder.Property(snapshot => snapshot.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(snapshot => snapshot.ContractId).HasColumnName("contract_id").IsRequired();
        builder.Property(snapshot => snapshot.Name)
            .HasColumnName("name")
            .HasMaxLength(ContractBillingSnapshotNameFormatter.MaxLength)
            .IsRequired();
        builder.Property(snapshot => snapshot.PeriodStart).HasColumnName("period_start").HasColumnType("date");
        builder.Property(snapshot => snapshot.PeriodEnd).HasColumnName("period_end").HasColumnType("date");
        builder.Property(snapshot => snapshot.ContractType).HasColumnName("contract_type").HasConversion<int>();
        builder.Property(snapshot => snapshot.Status).HasColumnName("status").HasConversion<int>();
        builder.Property(snapshot => snapshot.EngineVersion).HasColumnName("engine_version");
        builder.Property(snapshot => snapshot.InvoiceId).HasColumnName("invoice_id");
        builder.Property(snapshot => snapshot.ClaimInvoiceId).HasColumnName("claim_invoice_id");
        builder.Property(snapshot => snapshot.ExtraKmOverageInvoiceType)
            .HasColumnName("extra_km_overage_invoice_type")
            .HasConversion<int>();
        builder.Ignore(snapshot => snapshot.Invoice);
        builder.Property(snapshot => snapshot.FinalizedAt).HasColumnName("finalized_at").HasColumnType("timestamp with time zone");
        builder.Property(snapshot => snapshot.SubTotal).HasColumnName("sub_total").HasColumnType("decimal(18,2)");
        builder.Property(snapshot => snapshot.WorkingDaysAmount).HasColumnName("working_days_amount").HasColumnType("decimal(18,2)");
        builder.Property(snapshot => snapshot.ExtraWorkingDaysAmount).HasColumnName("extra_working_days_amount").HasColumnType("decimal(18,2)");
        builder.Property(snapshot => snapshot.ExtraKmsAmount).HasColumnName("extra_kms_amount").HasColumnType("decimal(18,2)");
        builder.Property(snapshot => snapshot.ExtraTripsAmount).HasColumnName("extra_trips_amount").HasColumnType("decimal(18,2)");
        builder.Property(snapshot => snapshot.WorkingDaysCount).HasColumnName("working_days_count");
        builder.Property(snapshot => snapshot.ExtraWorkingDaysCount).HasColumnName("extra_working_days_count");
        builder.Property(snapshot => snapshot.ExtraTripsCount).HasColumnName("extra_trips_count");
        builder.Property(snapshot => snapshot.ExtraKmsCount).HasColumnName("extra_kms_count").HasColumnType("decimal(18,2)");
        builder.Property(snapshot => snapshot.KmPoolJson).HasColumnName("km_pool_json").HasColumnType("jsonb");
        builder.Property(snapshot => snapshot.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(snapshot => snapshot.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(snapshot => snapshot.DeletedAt).HasColumnName("deleted_at");

        builder.HasOne(snapshot => snapshot.Contract)
            .WithMany()
            .HasForeignKey(snapshot => snapshot.ContractId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(snapshot => new { snapshot.ContractId, snapshot.PeriodStart, snapshot.PeriodEnd, snapshot.Status });
    }
}
