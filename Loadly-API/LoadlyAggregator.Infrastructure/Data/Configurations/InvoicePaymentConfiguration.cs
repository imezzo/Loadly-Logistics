using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class InvoicePaymentConfiguration : IEntityTypeConfiguration<InvoicePayment>
{
    public void Configure(EntityTypeBuilder<InvoicePayment> builder)
    {
        builder.ToTable("invoice_payments");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(p => p.InvoiceId)
            .HasColumnName("invoice_id")
            .IsRequired();
        builder.Property(p => p.BankTransferId)
            .HasColumnName("bank_transfer_id")
            .IsRequired();
        builder.Property(p => p.Amount)
            .HasColumnName("amount")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Property(p => p.AddedBy)
            .HasColumnName("added_by")
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

        builder.HasIndex(p => p.InvoiceId);
        builder.HasIndex(p => p.BankTransferId);
        builder.HasIndex(p => new { p.InvoiceId, p.BankTransferId })
            .IsUnique()
            .HasFilter("\"deleted_at\" IS NULL");

        builder.HasOne(p => p.Invoice)
            .WithMany(i => i.Payments)
            .HasForeignKey(p => p.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.BankTransfer)
            .WithMany(b => b.InvoicePayments)
            .HasForeignKey(p => p.BankTransferId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
