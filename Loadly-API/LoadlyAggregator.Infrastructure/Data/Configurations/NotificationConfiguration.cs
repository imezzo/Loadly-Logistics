using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(n => n.ModelId)
            .HasColumnName("model_id")
            .IsRequired();
        builder.Property(n => n.ModelType)
            .HasColumnName("model_type")
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(n => n.Title)
            .HasColumnName("title")
            .HasMaxLength(500)
            .IsRequired();
        builder.Property(n => n.Body)
            .HasColumnName("body")
            .HasColumnType("text")
            .IsRequired();
        builder.Property(n => n.EventType)
            .HasColumnName("event_type")
            .HasConversion<int>()
            .IsRequired(false);
        builder.Property(n => n.ResourceType)
            .HasColumnName("resource_type")
            .HasConversion<int>()
            .IsRequired(false);
        builder.Property(n => n.ResourceId)
            .HasColumnName("resource_id")
            .IsRequired(false);
        builder.Property(n => n.ClickAction)
            .HasColumnName("click_action")
            .HasMaxLength(100)
            .IsRequired(false);
        builder.Property(n => n.ReadAt)
            .HasColumnName("read_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
        builder.Property(n => n.BroadcastNotificationId)
            .HasColumnName("broadcast_notification_id")
            .IsRequired(false);
        builder.Property(n => n.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(n => n.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(n => n.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        builder.HasIndex(n => new { n.ModelId, n.ModelType });
        builder.HasIndex(n => n.CreatedAt);
        builder.HasIndex(n => n.EventType);
        builder.HasIndex(n => n.ReadAt);
        builder.HasIndex(n => new { n.ResourceType, n.ResourceId });
        builder.HasIndex(n => n.BroadcastNotificationId);

        builder.HasOne(n => n.BroadcastNotification)
            .WithMany()
            .HasForeignKey(n => n.BroadcastNotificationId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
