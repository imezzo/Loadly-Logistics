using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable("contracts");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(c => c.ReferenceNumber)
            .HasColumnName("reference_number")
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(c => c.ReferenceSequence)
            .HasColumnName("reference_sequence")
            .IsRequired();
        builder.Property(c => c.NameAr)
            .HasColumnName("name_ar")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(c => c.NameEn)
            .HasColumnName("name_en")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(c => c.CompanyId)
            .HasColumnName("company_id")
            .IsRequired();
        builder.Property(c => c.ContractType)
            .HasColumnName("contract_type_id")
            .HasConversion<int>()
            .IsRequired();
        builder.Property(c => c.Price)
            .HasColumnName("price")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Property(c => c.PaymentMethod)
            .HasColumnName("payment_method")
            .HasConversion<int>()
            .IsRequired();
        builder.Property(c => c.PaymentDueDays)
            .HasColumnName("payment_due_days")
            .HasColumnType("smallint")
            .IsRequired();
        builder.Property(c => c.BillingCycle)
            .HasColumnName("billing_cycle")
            .HasConversion<int>()
            .IsRequired();
        builder.Property(c => c.StartDate)
            .HasColumnName("start_date")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        builder.Property(c => c.EndDate)
            .HasColumnName("end_date")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        builder.Property(c => c.ConsumptionType)
            .HasColumnName("consumption_type")
            .HasConversion<int>()
            .IsRequired();
        builder.Property(c => c.CargoCommission)
            .HasColumnName("cargo_commission")
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0m)
            .IsRequired();
        builder.Property(c => c.MonthlyWorkingDays)
            .HasColumnName("monthly_working_days");
        builder.Property(c => c.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired()
            .HasDefaultValue(ContractStatus.DRAFT);
        builder.Property(c => c.TermsAndConditions)
            .HasColumnName("terms_and_conditions")
            .HasColumnType("text")
            .IsRequired(false);
        builder.Property(c => c.AddedBy)
            .HasColumnName("added_by")
            .IsRequired(false);
        builder.Property(c => c.SalesRepId)
            .HasColumnName("sales_rep_id")
            .IsRequired();
        builder.Property(c => c.CurrentApprovalStage)
            .HasColumnName("current_approval_stage")
            .HasConversion<int>()
            .IsRequired(false);
        builder.Property(c => c.DeactivationDate)
            .HasColumnName("deactivation_date")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
        builder.Property(c => c.RenewedFromContractId)
            .HasColumnName("renewed_from_contract_id")
            .IsRequired(false);
        builder.Property(c => c.AutoAssignMode)
            .HasColumnName("auto_assign_mode")
            .HasDefaultValue(false)
            .IsRequired();
        builder.Property(c => c.KmOverageMode)
            .HasColumnName("km_overage_mode")
            .HasConversion<int>()
            .IsRequired(false);
        builder.Property(c => c.VehiclesCount)
            .HasColumnName("vehicles_count")
            .IsRequired(false);
        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(c => c.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(c => c.ReferenceNumber).IsUnique();
        builder.HasIndex(c => c.NameAr);
        builder.HasIndex(c => c.NameEn);

        builder.HasIndex(c => c.ReferenceSequence);
        builder.HasIndex(c => c.CompanyId);
        builder.HasIndex(c => c.ContractType);
        builder.HasIndex(c => c.SalesRepId);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.RenewedFromContractId);

        builder.HasOne(c => c.Company)
            .WithMany()
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(c => c.SalesRep)
            .WithMany()
            .HasForeignKey(c => c.SalesRepId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(c => c.RenewedFromContract)
            .WithMany()
            .HasForeignKey(c => c.RenewedFromContractId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(c => c.Approvals)
            .WithOne(a => a.Contract)
            .HasForeignKey(a => a.ContractId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(c => c.StatusHistories)
            .WithOne(h => h.Contract)
            .HasForeignKey(h => h.ContractId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
