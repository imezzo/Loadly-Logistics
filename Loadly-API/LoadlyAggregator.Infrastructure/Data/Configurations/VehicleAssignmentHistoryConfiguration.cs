using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class VehicleAssignmentHistoryConfiguration : IEntityTypeConfiguration<VehicleAssignmentHistory>
{
    public void Configure(EntityTypeBuilder<VehicleAssignmentHistory> builder)
    {
        builder.ToTable("vehicle_assignment_histories");
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(h => h.VehicleId)
            .HasColumnName("vehicle_id")
            .IsRequired();
        builder.Property(h => h.Source)
            .HasColumnName("source")
            .HasConversion<int>()
            .IsRequired();
        builder.Property(h => h.PrevTailVehicleId)
            .HasColumnName("prev_tail_vehicle_id")
            .IsRequired(false);
        builder.Property(h => h.NewTailVehicleId)
            .HasColumnName("new_tail_vehicle_id")
            .IsRequired(false);
        builder.Property(h => h.PrevDriverId)
            .HasColumnName("prev_driver_id")
            .IsRequired(false);
        builder.Property(h => h.NewDriverId)
            .HasColumnName("new_driver_id")
            .IsRequired(false);
        builder.Property(h => h.AddedBy)
            .HasColumnName("added_by")
            .IsRequired(false);
        builder.Property(h => h.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(h => h.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(h => h.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(h => h.VehicleId);
        builder.HasIndex(h => h.CreatedAt);
        builder.HasIndex(h => new { h.VehicleId, h.CreatedAt });

        builder.HasOne(h => h.Vehicle)
            .WithMany()
            .HasForeignKey(h => h.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(h => h.PrevTailVehicle)
            .WithMany()
            .HasForeignKey(h => h.PrevTailVehicleId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(h => h.NewTailVehicle)
            .WithMany()
            .HasForeignKey(h => h.NewTailVehicleId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(h => h.PrevDriver)
            .WithMany()
            .HasForeignKey(h => h.PrevDriverId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(h => h.NewDriver)
            .WithMany()
            .HasForeignKey(h => h.NewDriverId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(h => h.AddedByAdmin)
            .WithMany()
            .HasForeignKey(h => h.AddedBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
