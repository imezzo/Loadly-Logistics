using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class VehicleDocumentConfiguration : IEntityTypeConfiguration<VehicleDocument>
{
    public void Configure(EntityTypeBuilder<VehicleDocument> builder)
    {
        builder.ToTable("vehicle_documents");
        builder.HasKey(vd => vd.Id);
        builder.Property(vd => vd.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(vd => vd.DocumentId)
            .HasColumnName("document_id")
            .IsRequired();
        builder.Property(vd => vd.VehicleId)
            .HasColumnName("vehicle_id")
            .IsRequired();
        builder.Property(vd => vd.Path)
            .HasColumnName("path")
            .HasMaxLength(500)
            .IsRequired(false);
        builder.Property(vd => vd.ExpiryDate)
            .HasColumnName("expiry_date")
            .IsRequired(false);
        builder.Property(vd => vd.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(DocumentVerificationStatus.PENDING)
            .HasSentinel((DocumentVerificationStatus)0);
        builder.Property(vd => vd.AddedBy)
            .HasColumnName("added_by")
            .IsRequired(false);
        builder.Property(vd => vd.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(vd => vd.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(vd => vd.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.HasIndex(vd => vd.DocumentId);
        builder.HasIndex(vd => vd.VehicleId);
        builder.HasIndex(vd => vd.Status);
        builder.HasOne(vd => vd.Document)
            .WithMany()
            .HasForeignKey(vd => vd.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(vd => vd.Vehicle)
            .WithMany(v => v.VehicleDocuments)
            .HasForeignKey(vd => vd.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
