using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class DeviceTokenConfiguration : IEntityTypeConfiguration<DeviceToken>
{
    public void Configure(EntityTypeBuilder<DeviceToken> builder)
    {
        builder.ToTable("device_tokens");
        builder.HasKey(dt => dt.Id);

        builder.Property(dt => dt.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");

        builder.Property(dt => dt.UserId)
            .HasColumnName("user_id")
            .HasColumnType("bigint")
            .IsRequired();

        builder.Property(dt => dt.UserType)
            .HasColumnName("user_type")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(dt => dt.FcmToken)
            .HasColumnName("fcm_token")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(dt => dt.DeviceId)
            .HasColumnName("device_id")
            .HasMaxLength(255);

        builder.Property(dt => dt.DeviceModel)
            .HasColumnName("device_model")
            .HasMaxLength(255);

        builder.Property(dt => dt.DeviceOs)
            .HasColumnName("device_os")
            .HasMaxLength(50);

        builder.Property(dt => dt.AppVersion)
            .HasColumnName("app_version")
            .HasMaxLength(50);

        builder.Property(dt => dt.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(dt => dt.LastUsedAt)
            .HasColumnName("last_used_at")
            .HasColumnType("timestamp with time zone");

        builder.Property(dt => dt.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(dt => dt.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(dt => dt.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(dt => new { dt.UserId, dt.UserType })
            .HasDatabaseName("IX_device_tokens_user_id_user_type");

        builder.HasIndex(dt => dt.FcmToken)
            .HasDatabaseName("IX_device_tokens_fcm_token");

        builder.HasIndex(dt => dt.IsActive)
            .HasDatabaseName("IX_device_tokens_is_active")
            .HasFilter("\"is_active\" = true");

        builder.HasIndex(dt => new { dt.UserId, dt.UserType, dt.FcmToken })
            .IsUnique()
            .HasDatabaseName("IX_device_tokens_user_type_token_unique");
    }
}
