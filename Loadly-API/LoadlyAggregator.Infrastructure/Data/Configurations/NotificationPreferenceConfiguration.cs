using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
{
    public void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.ToTable("notification_preferences");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.ModelId)
            .HasColumnName("model_id")
            .IsRequired();

        builder.Property(e => e.ModelType)
            .HasColumnName("model_type")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Channel)
            .HasColumnName("channel")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(e => e.EventType)
            .HasColumnName("event_type")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(e => e.IsEnabled)
            .HasColumnName("is_enabled")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(e => e.DeletedAt)
            .HasColumnName("deleted_at");

        builder.HasIndex(e => new { e.ModelId, e.ModelType, e.Channel, e.EventType })
            .HasDatabaseName("idx_notification_preferences_model_channel_event")
            .IsUnique();

        builder.HasIndex(e => new { e.ModelId, e.ModelType })
            .HasDatabaseName("idx_notification_preferences_model");

        builder.HasQueryFilter(e => e.DeletedAt == null);
    }
}
