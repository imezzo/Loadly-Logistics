using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class RecordingConfiguration : IEntityTypeConfiguration<Recording>
{
    public void Configure(EntityTypeBuilder<Recording> builder)
    {
        builder.ToTable("recordings");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(r => r.RecordableType)
            .HasColumnName("recordable_type")
            .HasConversion<int>()
            .IsRequired();
        builder.Property(r => r.RecordableId)
            .HasColumnName("recordable_id")
            .IsRequired();
        builder.Property(r => r.RecordableEventType)
            .HasColumnName("recordable_event_type")
            .HasConversion<int>()
            .IsRequired();
        builder.Property(r => r.Data)
            .HasColumnName("data")
            .HasColumnType("jsonb");
        builder.Property(r => r.AddedBy)
            .HasColumnName("added_by")
            .IsRequired(false);
        builder.Property(r => r.AddedByModelType)
            .HasColumnName("added_by_model_type")
            .HasMaxLength(32)
            .IsRequired(false);
        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(r => r.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(r => new { r.RecordableId, r.RecordableType, r.CreatedAt });
    }
}
