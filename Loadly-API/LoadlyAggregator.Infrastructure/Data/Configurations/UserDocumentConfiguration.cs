using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class UserDocumentConfiguration : IEntityTypeConfiguration<UserDocument>
{
    public void Configure(EntityTypeBuilder<UserDocument> builder)
    {
        builder.ToTable("user_documents");
        builder.HasKey(ud => ud.Id);
        builder.Property(ud => ud.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(ud => ud.DocumentId)
            .HasColumnName("document_id")
            .IsRequired();
        builder.Property(ud => ud.ModelType)
            .HasColumnName("model_type")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(ud => ud.ModelId)
            .HasColumnName("model_id")
            .IsRequired();
        builder.Property(ud => ud.ExpiryDate)
            .HasColumnName("expiry_date")
            .HasColumnType("timestamp with time zone");
        builder.Property(ud => ud.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(DocumentVerificationStatus.PENDING);
        builder.Property(ud => ud.AddedBy)
            .HasColumnName("added_by");
        builder.Property(ud => ud.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(ud => ud.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(ud => ud.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.HasIndex(ud => ud.DocumentId);
        builder.HasIndex(ud => new { ud.ModelId, ud.ModelType });
        builder.HasIndex(ud => ud.Status);
        builder.HasIndex(ud => ud.ExpiryDate);
        builder.HasOne(ud => ud.Document)
            .WithMany()
            .HasForeignKey(ud => ud.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
