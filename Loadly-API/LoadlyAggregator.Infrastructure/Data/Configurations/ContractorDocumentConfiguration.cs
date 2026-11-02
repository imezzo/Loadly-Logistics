using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ContractorDocumentConfiguration : IEntityTypeConfiguration<ContractorDocument>
{
    public void Configure(EntityTypeBuilder<ContractorDocument> builder)
    {
        builder.ToTable("contractor_documents");
        builder.HasKey(cd => cd.Id);
        builder.Property(cd => cd.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(cd => cd.DocumentId)
            .HasColumnName("document_id")
            .IsRequired();
        builder.Property(cd => cd.ContractorId)
            .HasColumnName("contractor_id")
            .IsRequired();
        builder.Property(cd => cd.Path)
            .HasColumnName("path")
            .HasMaxLength(500)
            .IsRequired(false);
        builder.Property(cd => cd.ExpiryDate)
            .HasColumnName("expiry_date")
            .IsRequired(false);
        builder.Property(cd => cd.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(DocumentVerificationStatus.PENDING)
            .HasSentinel((DocumentVerificationStatus)0);
        builder.Property(cd => cd.AddedBy)
            .HasColumnName("added_by")
            .IsRequired(false);
        builder.Property(cd => cd.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(cd => cd.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(cd => cd.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.HasIndex(cd => cd.DocumentId);
        builder.HasIndex(cd => cd.ContractorId);
        builder.HasIndex(cd => cd.Status);
        builder.HasOne(cd => cd.Document)
            .WithMany()
            .HasForeignKey(cd => cd.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(cd => cd.Contractor)
            .WithMany(c => c.ContractorDocuments)
            .HasForeignKey(cd => cd.ContractorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
