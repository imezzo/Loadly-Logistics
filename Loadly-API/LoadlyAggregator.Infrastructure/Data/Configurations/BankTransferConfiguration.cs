using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class BankTransferConfiguration : IEntityTypeConfiguration<BankTransfer>
{
    public void Configure(EntityTypeBuilder<BankTransfer> builder)
    {
        builder.ToTable("bank_transfers");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(b => b.Amount)
            .HasColumnName("amount")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Property(b => b.IssueDate)
            .HasColumnName("issue_date")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
        builder.Property(b => b.PaymentMethod)
            .HasColumnName("payment_method")
            .HasConversion<int>()
            .IsRequired();
        builder.Property(b => b.ExternalPaymentReferenceNumber)
            .HasColumnName("external_payment_reference_number")
            .HasMaxLength(100)
            .IsRequired(false);
        builder.Property(b => b.Attachment)
            .HasColumnName("attachment")
            .HasMaxLength(500)
            .IsRequired(false);
        builder.Property(b => b.PayerId)
            .HasColumnName("payer_id")
            .IsRequired(false);
        builder.Property(b => b.PayerType)
            .HasColumnName("payer_type")
            .HasMaxLength(255)
            .IsRequired(false);
        builder.Property(b => b.AddedBy)
            .HasColumnName("added_by")
            .IsRequired(false);
        builder.HasIndex(b => new { b.PayerId, b.PayerType });

        builder.HasOne(b => b.Company)
            .WithMany()
            .HasForeignKey(b => b.PayerId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(b => b.Lead)
            .WithMany()
            .HasForeignKey(b => b.PayerId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(b => b.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(b => b.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(b => b.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
    }
}
