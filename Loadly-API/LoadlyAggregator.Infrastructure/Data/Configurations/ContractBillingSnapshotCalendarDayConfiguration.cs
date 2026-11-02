using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ContractBillingSnapshotCalendarDayConfiguration : IEntityTypeConfiguration<ContractBillingSnapshotCalendarDay>
{
    public void Configure(EntityTypeBuilder<ContractBillingSnapshotCalendarDay> builder)
    {
        builder.ToTable("contract_billing_snapshot_calendar_days");
        builder.HasKey(day => day.Id);
        builder.Property(day => day.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(day => day.SnapshotId).HasColumnName("snapshot_id");
        builder.Property(day => day.ContractSlotId).HasColumnName("contract_slot_id");
        builder.Property(day => day.Date).HasColumnName("date").HasColumnType("date");
        builder.Property(day => day.Disposition).HasColumnName("disposition").HasConversion<int>();
        builder.Property(day => day.DispositionSource).HasColumnName("disposition_source").HasConversion<int>();
        builder.Property(day => day.TripIds).HasColumnName("trip_ids").HasColumnType("bigint[]").IsRequired();
        builder.Property(day => day.TripCountSource).HasColumnName("trip_count_source").HasConversion<int>();
        builder.Property(day => day.WorkingDayIndex).HasColumnName("working_day_index");
        builder.Property(day => day.IsExtra).HasColumnName("is_extra");
        builder.Property(day => day.FacilityType).HasColumnName("facility_type").HasConversion<int?>();
        builder.Property(day => day.FacilityId).HasColumnName("facility_id");
        builder.Property(day => day.OverrideReason).HasColumnName("override_reason").HasMaxLength(500);
        builder.Property(day => day.ChangedBy).HasColumnName("changed_by");
        builder.Property(day => day.ChangedAt).HasColumnName("changed_at");
        builder.Property(day => day.CreatedAt).HasColumnName("created_at");
        builder.Property(day => day.UpdatedAt).HasColumnName("updated_at");
        builder.Property(day => day.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(day => new { day.SnapshotId, day.ContractSlotId, day.Date }).IsUnique()
            .HasFilter("\"deleted_at\" IS NULL");

        builder.HasOne(day => day.Snapshot)
            .WithMany(snapshot => snapshot.CalendarDays)
            .HasForeignKey(day => day.SnapshotId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(day => day.ContractSlot)
            .WithMany()
            .HasForeignKey(day => day.ContractSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(day => day.VehicleFacility)
            .WithMany()
            .HasForeignKey(day => day.FacilityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
