using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class DriverLocationConfiguration : IEntityTypeConfiguration<DriverLocation>
{
    public void Configure(EntityTypeBuilder<DriverLocation> builder)
    {
        builder.ToTable("driver_locations");

        builder.HasKey(dl => dl.Id);
        builder.Property(dl => dl.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(dl => dl.DriverId)
            .HasColumnName("driver_id")
            .IsRequired();

        builder.Property(dl => dl.Latitude)
            .HasColumnName("latitude")
            .HasColumnType("decimal(10,8)")
            .IsRequired();

        builder.Property(dl => dl.Longitude)
            .HasColumnName("longitude")
            .HasColumnType("decimal(11,8)")
            .IsRequired();

        builder.Property(dl => dl.Accuracy)
            .HasColumnName("accuracy")
            .HasColumnType("decimal(10,2)")
            .IsRequired(false);

        builder.Property(dl => dl.Heading)
            .HasColumnName("heading")
            .HasColumnType("decimal(5,2)")
            .IsRequired(false);

        builder.Property(dl => dl.Speed)
            .HasColumnName("speed")
            .HasColumnType("decimal(10,2)")
            .IsRequired(false);

        builder.Property(dl => dl.RecordedAt)
            .HasColumnName("recorded_at")
            .IsRequired();

        builder.Property(dl => dl.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.Property(dl => dl.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(dl => dl.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasIndex(dl => dl.DriverId)
            .IsUnique();

        builder.HasOne(dl => dl.Driver)
            .WithOne(d => d.DriverLocation)
            .HasForeignKey<DriverLocation>(dl => dl.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
