using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class OdometerScanLogConfiguration : IEntityTypeConfiguration<OdometerScanLog>
{
    public void Configure(EntityTypeBuilder<OdometerScanLog> builder)
    {
        builder.ToTable("odometer_scan_logs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.ModelId)
            .HasColumnName("model_id")
            .HasColumnType("bigint");

        builder.Property(x => x.ModelType)
            .HasColumnName("model_type")
            .HasMaxLength(255);

        builder.Property(x => x.TripId)
            .HasColumnName("trip_id")
            .HasColumnType("bigint");

        builder.Property(x => x.ScanContext)
            .HasColumnName("scan_context")
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(OdometerScanContext.Suggest);

        builder.Property(x => x.ImagePath)
            .HasColumnName("image_path")
            .HasMaxLength(1024);

        builder.Property(x => x.SuggestedReading)
            .HasColumnName("suggested_reading")
            .HasColumnType("integer");

        builder.Property(x => x.Unit)
            .HasColumnName("unit")
            .HasMaxLength(20);

        builder.Property(x => x.IsDigital)
            .HasColumnName("is_digital");

        builder.Property(x => x.ReadingQuality)
            .HasColumnName("reading_quality")
            .HasMaxLength(20);

        builder.Property(x => x.IsScreen)
            .HasColumnName("is_screen")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(OdometerScanStatus.Failure);

        builder.Property(x => x.ErrorMessage)
            .HasColumnName("error_message")
            .HasColumnType("text");

        builder.Property(x => x.Provider)
            .HasColumnName("provider")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Model)
            .HasColumnName("model")
            .HasMaxLength(100);

        builder.Property(x => x.TotalTokens)
            .HasColumnName("total_tokens")
            .HasColumnType("integer");

        builder.Property(x => x.LatencyMs)
            .HasColumnName("latency_ms")
            .HasColumnType("integer");

        builder.Property(x => x.RawResponse)
            .HasColumnName("raw_response")
            .HasColumnType("text");

        builder.Property(x => x.CorrelationId)
            .HasColumnName("correlation_id")
            .HasMaxLength(CorrelationIdConstants.MaxLength);

        builder.Property(x => x.PreviousScanLogId)
            .HasColumnName("previous_scan_log_id")
            .HasColumnType("bigint");

        builder.HasIndex(x => new { x.ModelType, x.ModelId });
        builder.HasIndex(x => new { x.TripId, x.CreatedAt });
        builder.HasIndex(x => x.CorrelationId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedAt);
    }
}
