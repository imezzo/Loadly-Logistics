using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class DocumentStatusConfiguration : IEntityTypeConfiguration<LookupDocumentStatus>
{
    public void Configure(EntityTypeBuilder<LookupDocumentStatus> builder)
    {
        builder.ToTable("lookup_document_status");
        builder.HasKey(ds => ds.Id);
        builder.Property(ds => ds.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(ds => ds.NameAr)
            .HasColumnName("name_ar")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(ds => ds.NameEn)
            .HasColumnName("name_en")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(ds => ds.AddedBy)
            .HasColumnName("added_by");
        builder.Property(ds => ds.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(ds => ds.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(ds => ds.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.HasIndex(ds => ds.NameAr).IsUnique();
        builder.HasIndex(ds => ds.NameEn).IsUnique();
    }
}
