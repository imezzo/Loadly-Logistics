using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class DriverDocumentConfiguration : IEntityTypeConfiguration<DriverDocument>
{
    public void Configure(EntityTypeBuilder<DriverDocument> builder)
    {
        builder.ToTable("driver_documents");
        builder.HasKey(dd => dd.Id);
        builder.Property(dd => dd.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(dd => dd.DocumentId)
            .HasColumnName("document_id")
            .IsRequired();
        builder.Property(dd => dd.DriverId)
            .HasColumnName("driver_id")
            .IsRequired();
        builder.Property(dd => dd.Path)
            .HasColumnName("path")
            .HasMaxLength(500)
            .IsRequired(false);
        builder.Property(dd => dd.ExpiryDate)
            .HasColumnName("expiry_date")
            .IsRequired(false);
        builder.Property(dd => dd.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(DocumentVerificationStatus.PENDING)
            .HasSentinel((DocumentVerificationStatus)0);
        builder.Property(dd => dd.AddedBy)
            .HasColumnName("added_by")
            .IsRequired(false);
        builder.Property(dd => dd.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(dd => dd.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(dd => dd.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.HasIndex(dd => dd.DocumentId);
        builder.HasIndex(dd => dd.DriverId);
        builder.HasIndex(dd => dd.Status);
        builder.HasOne(dd => dd.Document)
            .WithMany()
            .HasForeignKey(dd => dd.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(dd => dd.Driver)
            .WithMany(d => d.DriverDocuments)
            .HasForeignKey(dd => dd.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
