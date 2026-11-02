using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class TripRouteSyncConfiguration : IEntityTypeConfiguration<TripRouteSync>
{
    public void Configure(EntityTypeBuilder<TripRouteSync> builder)
    {
        builder.ToTable("trip_route_syncs");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.TripId)
            .HasColumnName("trip_id")
            .IsRequired();

        builder.Property(s => s.DriverId)
            .HasColumnName("driver_id")
            .IsRequired();

        builder.Property(s => s.DistanceMeters)
            .HasColumnName("distance_meters")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(s => s.StartTime)
            .HasColumnName("start_time")
            .IsRequired();

        builder.Property(s => s.EndTime)
            .HasColumnName("end_time")
            .IsRequired();

        builder.Property(s => s.GpsPacketCount)
            .HasColumnName("gps_packet_count")
            .IsRequired();

        builder.Property(s => s.PointCount)
            .HasColumnName("point_count")
            .IsRequired();

        builder.Property(s => s.Payload)
            .HasColumnName("payload")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(s => s.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(s => s.TripId);
        builder.HasIndex(s => new { s.TripId, s.CreatedAt });

        builder.HasOne<Trip>()
            .WithMany()
            .HasForeignKey(s => s.TripId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
