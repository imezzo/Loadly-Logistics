using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class BroadcastNotificationConfiguration : IEntityTypeConfiguration<BroadcastNotification>
{
    public void Configure(EntityTypeBuilder<BroadcastNotification> builder)
    {
        builder.ToTable("broadcast_notifications");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(b => b.AddedBy)
            .HasColumnName("added_by")
            .IsRequired();
        builder.Property(b => b.Channel)
            .HasColumnName("channel")
            .IsRequired()
            .HasConversion<int>();
        builder.Property(b => b.Subject)
            .HasColumnName("subject")
            .HasMaxLength(500)
            .IsRequired(false);
        builder.Property(b => b.Body)
            .HasColumnName("body")
            .HasColumnType("text")
            .IsRequired();
        builder.Property(b => b.ModelType)
            .HasColumnName("model_type")
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(b => b.ModelIds)
            .HasColumnName("model_ids")
            .HasColumnType("text")
            .IsRequired(false);
        builder.Property(b => b.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(NotificationStatus.PENDING);
        builder.Property(b => b.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        builder.Property(b => b.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        builder.Property(b => b.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
        builder.Property(b => b.PublicToken)
            .HasColumnName("public_token")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.HasIndex(b => b.AddedBy);
        builder.HasIndex(b => b.Status);
        builder.HasIndex(b => b.Channel);
        builder.HasIndex(b => b.ModelType);
        builder.HasIndex(b => b.CreatedAt);
        builder.HasIndex(b => b.PublicToken)
            .IsUnique()
            .HasFilter("public_token IS NOT NULL");
    }
}
