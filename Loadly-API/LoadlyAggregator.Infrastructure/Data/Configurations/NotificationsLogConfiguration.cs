using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class NotificationsLogConfiguration : IEntityTypeConfiguration<NotificationsLog>
{
    public void Configure(EntityTypeBuilder<NotificationsLog> builder)
    {
        builder.ToTable("notifications_log");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");
        builder.Property(n => n.Recipient)
            .HasColumnName("recipient")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(n => n.Channel)
            .HasColumnName("recipient_type")
            .IsRequired()
            .HasConversion<int>();
        builder.Property(n => n.Response)
            .HasColumnName("response")
            .HasColumnType("text");
        builder.Property(n => n.Body)
            .HasColumnName("body")
            .IsRequired()
            .HasColumnType("text");
        builder.Property(n => n.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(NotificationStatus.PENDING);
        builder.Property(n => n.ModelId)
            .HasColumnName("model_id")
            .HasColumnType("bigint");
        builder.Property(n => n.EventType)
            .HasColumnName("event_type")
            .HasConversion<int>()
            .IsRequired(false);
        builder.Property(n => n.ModelType)
            .HasColumnName("model_type")
            .HasMaxLength(255);
        builder.Property(n => n.BroadcastNotificationId)
            .HasColumnName("broadcast_notification_id")
            .HasColumnType("bigint")
            .IsRequired(false);

        builder.Property(n => n.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        builder.Property(n => n.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        builder.Property(n => n.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(n => n.Channel);
        builder.HasIndex(n => n.Status);
        builder.HasIndex(n => n.Recipient);
        builder.HasIndex(n => n.CreatedAt);
        builder.HasIndex(n => new { n.Channel, n.Status, n.CreatedAt });
        builder.HasIndex(n => new { n.ModelType, n.ModelId });
        builder.HasIndex(n => n.EventType);
        builder.HasIndex(n => n.BroadcastNotificationId);
    }
}
