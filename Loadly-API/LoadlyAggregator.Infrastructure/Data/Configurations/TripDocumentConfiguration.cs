using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class TripDocumentConfiguration : IEntityTypeConfiguration<TripDocument>
{
    public void Configure(EntityTypeBuilder<TripDocument> builder)
    {
        builder.ToTable("trip_documents");
        builder.HasKey(td => td.Id);

        builder.Property(td => td.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(td => td.TripId)
            .HasColumnName("trip_id")
            .IsRequired();

        builder.Property(td => td.DocumentId)
            .HasColumnName("document_id")
            .IsRequired();

        builder.Property(td => td.Paths)
            .HasColumnName("paths")
            .HasColumnType("text[]")
            .IsRequired(false);

        builder.Property(td => td.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired(false);

        builder.Property(td => td.Note)
            .HasColumnName("note")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(td => td.AddedBy)
            .HasColumnName("added_by");

        builder.Property(td => td.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.Property(td => td.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(td => td.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasIndex(td => td.TripId);
        builder.HasIndex(td => td.DocumentId);
        builder.HasIndex(td => td.Status);

        // Enforce the projector's invariant: at most one current row per (trip, document kind).
        // Filtered to non-deleted rows so soft-deleted history doesn't block re-insertion.
        builder.HasIndex(td => new { td.TripId, td.DocumentId })
            .IsUnique()
            .HasFilter("\"deleted_at\" IS NULL");

        builder.HasOne(td => td.Trip)
            .WithMany(t => t.TripDocuments)
            .HasForeignKey(td => td.TripId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(td => td.Document)
            .WithMany()
            .HasForeignKey(td => td.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
