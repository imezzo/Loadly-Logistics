using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class WaypointConfiguration : IEntityTypeConfiguration<Waypoint>
{
    public void Configure(EntityTypeBuilder<Waypoint> builder)
    {
        builder.ToTable("waypoints");

        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(w => w.GooglePlaceId)
            .HasColumnName("google_place_id")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(w => w.Latitude)
            .HasColumnName("latitude")
            .HasColumnType("decimal(10,8)")
            .IsRequired(false);

        builder.Property(w => w.Longitude)
            .HasColumnName("longitude")
            .HasColumnType("decimal(11,8)")
            .IsRequired(false);

        builder.Property(w => w.AddressName)
            .HasColumnName("address_name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(w => w.AddedBy)
            .HasColumnName("added_by")
            .IsRequired(false);

        builder.Property(w => w.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(w => w.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(w => w.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
    }
}
