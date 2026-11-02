using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ContractorPayoutConfiguration : IEntityTypeConfiguration<ContractorPayout>
{
    public void Configure(EntityTypeBuilder<ContractorPayout> builder)
    {
        builder.ToTable("contractor_payouts");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(p => p.PayoutNumber)
            .HasColumnName("payout_number")
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(p => p.Sequence)
            .HasColumnName("sequence")
            .IsRequired(false);
        builder.Property(p => p.ContractorId)
            .HasColumnName("contractor_id")
            .IsRequired();
        builder.Property(p => p.TotalAmount)
            .HasColumnName("total_amount")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Property(p => p.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(ContractorPayoutStatus.Pending)
            .HasSentinel((ContractorPayoutStatus)0);
        builder.Property(p => p.AddedBy)
            .HasColumnName("added_by")
            .IsRequired(false);
        builder.Property(p => p.CancelledBy)
            .HasColumnName("cancelled_by")
            .IsRequired(false);
        builder.Property(p => p.CancelledAt)
            .HasColumnName("cancelled_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
        builder.Property(p => p.CancellationReason)
            .HasColumnName("cancellation_reason")
            .HasMaxLength(500)
            .IsRequired(false);
        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(p => p.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(p => p.PayoutNumber)
            .IsUnique()
            .HasFilter("\"deleted_at\" IS NULL");

        builder.HasIndex(p => p.ContractorId);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => new { p.ContractorId, p.Status });

        builder.HasOne(p => p.Contractor)
            .WithMany(c => c.Payouts)
            .HasForeignKey(p => p.ContractorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
