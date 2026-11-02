using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ContractDocumentConfiguration : IEntityTypeConfiguration<ContractDocument>
{
    public void Configure(EntityTypeBuilder<ContractDocument> builder)
    {
        builder.ToTable("contract_documents");
        builder.HasKey(cd => cd.Id);
        builder.Property(cd => cd.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(cd => cd.ContractId)
            .HasColumnName("contract_id")
            .IsRequired();
        builder.Property(cd => cd.DocumentId)
            .HasColumnName("document_id")
            .IsRequired();
        builder.Property(cd => cd.Path)
            .HasColumnName("path")
            .HasMaxLength(500)
            .IsRequired(false);
        builder.Property(cd => cd.ExpiryDate)
            .HasColumnName("expiry_date")
            .HasColumnType("timestamp with time zone");
        builder.Property(cd => cd.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(DocumentVerificationStatus.PENDING);
        builder.Property(cd => cd.AddedBy)
            .HasColumnName("added_by");
        builder.Property(cd => cd.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(cd => cd.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(cd => cd.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.HasIndex(cd => cd.ContractId);
        builder.HasIndex(cd => cd.DocumentId);
        builder.HasIndex(cd => cd.Status);
        builder.HasIndex(cd => cd.ExpiryDate);
        builder.HasOne(cd => cd.Contract)
            .WithMany(c => c.ContractDocuments)
            .HasForeignKey(cd => cd.ContractId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(cd => cd.Document)
            .WithMany()
            .HasForeignKey(cd => cd.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
