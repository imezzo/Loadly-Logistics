using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");
        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Id)
            .HasColumnName("id")
            .HasColumnType("bigint");
        builder.Property(rt => rt.TokenHash)
            .HasColumnName("token_hash")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(rt => rt.ModelId)
            .HasColumnName("model_id")
            .HasColumnType("bigint")
            .IsRequired();
        builder.Property(rt => rt.ModelType)
            .HasColumnName("model_type")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(rt => rt.TokenFamilyId)
            .HasColumnName("token_family_id")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(rt => rt.DeviceInfo)
            .HasColumnName("device_info")
            .HasColumnType("text");
        builder.Property(rt => rt.IpAddress)
            .HasColumnName("ip_address")
            .HasMaxLength(45);
        builder.Property(rt => rt.UserAgent)
            .HasColumnName("user_agent")
            .HasMaxLength(500);
        builder.Property(rt => rt.LastUsedAt)
            .HasColumnName("last_used_at")
            .HasColumnType("timestamp with time zone");
        builder.Property(rt => rt.SessionName)
            .HasColumnName("session_name")
            .HasMaxLength(100);
        builder.Property(rt => rt.ExpiresAt)
            .HasColumnName("expires_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        builder.Property(rt => rt.RevokedAt)
            .HasColumnName("revoked_at")
            .HasColumnType("timestamp with time zone");
        builder.Property(rt => rt.ReplacedByTokenHash)
            .HasColumnName("replaced_by_token_hash")
            .HasMaxLength(255);
        builder.Property(rt => rt.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        builder.Property(rt => rt.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        builder.Property(rt => rt.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(rt => rt.TokenHash)
            .IsUnique()
            .HasDatabaseName("IX_refresh_tokens_token_hash");
        builder.HasIndex(rt => new { rt.ModelId, rt.ModelType })
            .HasDatabaseName("IX_refresh_tokens_model_id_model_type");
        builder.HasIndex(rt => rt.TokenFamilyId)
            .HasDatabaseName("IX_refresh_tokens_token_family_id");
        builder.HasIndex(rt => rt.ExpiresAt)
            .HasDatabaseName("IX_refresh_tokens_expires_at");
    }
}
