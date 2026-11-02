using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class InvoiceDetailConfiguration : IEntityTypeConfiguration<InvoiceDetail>
{
    public void Configure(EntityTypeBuilder<InvoiceDetail> builder)
    {
        builder.ToTable("invoice_details");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(d => d.InvoiceId)
            .HasColumnName("invoice_id")
            .IsRequired();
        builder.Property(d => d.AddedBy)
            .HasColumnName("added_by")
            .IsRequired(false);
        builder.Property(d => d.UpdatedBy)
            .HasColumnName("updated_by")
            .IsRequired(false);
        builder.Property(d => d.PaidBy)
            .HasColumnName("paid_by")
            .IsRequired(false);
        builder.Property(d => d.PaidAt)
            .HasColumnName("paid_at")
            .IsRequired(false);
        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(d => d.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(d => d.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(d => d.InvoiceId)
            .IsUnique()
            .HasFilter("\"deleted_at\" IS NULL");

        builder.HasOne(d => d.Invoice)
            .WithOne(i => i.InvoiceDetail)
            .HasForeignKey<InvoiceDetail>(d => d.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.AddedByAdmin)
            .WithMany()
            .HasForeignKey(d => d.AddedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.UpdatedByAdmin)
            .WithMany()
            .HasForeignKey(d => d.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.PaidByAdmin)
            .WithMany()
            .HasForeignKey(d => d.PaidBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
