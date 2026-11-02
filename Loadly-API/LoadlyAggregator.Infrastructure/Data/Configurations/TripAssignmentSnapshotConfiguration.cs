using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class TripVehicleAssignmentSnapshotConfiguration : IEntityTypeConfiguration<TripVehicleAssignmentSnapshot>
{
    public void Configure(EntityTypeBuilder<TripVehicleAssignmentSnapshot> builder)
    {
        builder.ToTable("trip_vehicle_assignment_snapshot");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(s => s.TripId)
            .HasColumnName("trip_id")
            .IsRequired();
        builder.Property(s => s.HeadPlateNo)
            .HasColumnName("head_plate_no")
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(s => s.HeadPlateLetters)
            .HasColumnName("head_plate_letters")
            .HasMaxLength(10)
            .IsRequired();
        builder.Property(s => s.TailPlateNo)
            .HasColumnName("tail_plate_no")
            .HasMaxLength(50)
            .IsRequired(false);
        builder.Property(s => s.TailPlateLetters)
            .HasColumnName("tail_plate_letters")
            .HasMaxLength(10)
            .IsRequired(false);
        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(s => s.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(s => s.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(s => s.AddedBy)
            .HasColumnName("added_by")
            .IsRequired(false);

        builder.HasIndex(s => s.TripId)
            .IsUnique();

        builder.HasIndex(s => new { s.HeadPlateNo, s.HeadPlateLetters });
        builder.HasIndex(s => new { s.TailPlateNo, s.TailPlateLetters });

        builder.HasOne(s => s.Trip)
            .WithOne(t => t.VehicleAssignmentSnapshot)
            .HasForeignKey<TripVehicleAssignmentSnapshot>(s => s.TripId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(s => s.AddedByAdmin)
            .WithMany()
            .HasForeignKey(s => s.AddedBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
