using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    private static readonly JsonSerializerOptions JsonOptions = new();

    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");
        builder.Property(a => a.EventType)
            .HasColumnName("event_type")
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(a => a.EntityType)
            .HasColumnName("entity_type")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(a => a.EntityId)
            .HasColumnName("entity_id")
            .HasColumnType("bigint");
        builder.Property(a => a.UserId)
            .HasColumnName("user_id")
            .HasColumnType("bigint");
        builder.Property(a => a.UserType)
            .HasColumnName("user_type")
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(a => a.OldValues)
            .HasColumnName("old_values")
            .HasColumnType("jsonb");
        builder.Property(a => a.NewValues)
            .HasColumnName("new_values")
            .HasColumnType("jsonb");
        builder.Property(a => a.ChangeSummary)
            .HasColumnName("change_summary")
            .HasColumnType("jsonb")
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, JsonOptions),
                v => string.IsNullOrEmpty(v) ? null : JsonSerializer.Deserialize<List<string>>(v, JsonOptions))
            .Metadata.SetValueComparer(new ValueComparer<List<string>?>(
                (a, b) => (a == null && b == null) || (a != null && b != null && a.SequenceEqual(b)),
                v => v == null ? 0 : v.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
                v => v == null ? null : v.ToList()));
        builder.Property(a => a.IpAddress)
            .HasColumnName("ip_address")
            .IsRequired()
            .HasMaxLength(64);
        builder.Property(a => a.UserAgent)
            .HasColumnName("user_agent")
            .IsRequired()
            .HasMaxLength(1024);
        builder.Property(a => a.CorrelationId)
            .HasColumnName("correlation_id")
            .HasMaxLength(CorrelationIdConstants.MaxLength);
        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at");
        builder.HasIndex(a => a.CorrelationId);
        builder.HasIndex(a => new { a.UserId, a.UserType });
        builder.HasIndex(a => new { a.EntityType, a.EntityId });
        builder.HasIndex(a => a.CreatedAt);
        builder.HasIndex(a => new { a.EntityType, a.CreatedAt });
    }
}
