using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class BroadcastNotificationAttachmentConfiguration : IEntityTypeConfiguration<BroadcastNotificationAttachment>
{
    public void Configure(EntityTypeBuilder<BroadcastNotificationAttachment> builder)
    {
        builder.ToTable("broadcast_notification_attachments");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(a => a.BroadcastNotificationId)
            .HasColumnName("broadcast_notification_id")
            .IsRequired();
        builder.Property(a => a.Path)
            .HasColumnName("path")
            .HasColumnType("text")
            .IsRequired();
        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        builder.Property(a => a.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        builder.HasIndex(a => a.BroadcastNotificationId);

        builder.HasOne(a => a.BroadcastNotification)
            .WithMany(n => n.Attachments)
            .HasForeignKey(a => a.BroadcastNotificationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
