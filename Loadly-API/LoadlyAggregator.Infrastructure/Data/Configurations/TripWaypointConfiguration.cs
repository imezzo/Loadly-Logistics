using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class TripWaypointConfiguration : IEntityTypeConfiguration<TripWaypoint>
{
    public void Configure(EntityTypeBuilder<TripWaypoint> builder)
    {
        builder.ToTable("trip_waypoints");

        builder.HasKey(tw => tw.Id);
        builder.Property(tw => tw.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(tw => tw.TripId)
            .HasColumnName("trip_id")
            .IsRequired();

        builder.Property(tw => tw.Sequence)
            .HasColumnName("sequence")
            .IsRequired();

        builder.Property(tw => tw.StopType)
            .HasColumnName("stop_type")
            .HasConversion<int>()
            .HasDefaultValue(TripWaypointStopType.Pickup)
            .HasSentinel((TripWaypointStopType)0);

        builder.Property(tw => tw.GooglePlaceId)
            .HasColumnName("google_place_id")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(tw => tw.Latitude)
            .HasColumnName("latitude")
            .HasColumnType("decimal(10,8)")
            .IsRequired(false);

        builder.Property(tw => tw.Longitude)
            .HasColumnName("longitude")
            .HasColumnType("decimal(11,8)")
            .IsRequired(false);

        builder.Property(tw => tw.AddressName)
            .HasColumnName("address_name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(tw => tw.TaskDetails)
            .HasColumnName("task_details")
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(tw => tw.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(TripWaypointStatus.Pending)
            .HasSentinel((TripWaypointStatus)0);

        builder.Property(tw => tw.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(tw => tw.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(tw => tw.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(tw => tw.TripId);
        builder.HasIndex(tw => new { tw.TripId, tw.Sequence });

        builder.HasOne(tw => tw.Trip)
            .WithMany(t => t.Waypoints)
            .HasForeignKey(tw => tw.TripId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
