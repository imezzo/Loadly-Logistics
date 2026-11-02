using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ContractBillingSnapshotCommercialOverrideConfiguration
    : IEntityTypeConfiguration<ContractBillingSnapshotCommercialOverride>
{
    public void Configure(EntityTypeBuilder<ContractBillingSnapshotCommercialOverride> builder)
    {
        builder.ToTable("contract_billing_snapshot_commercial_overrides");
        builder.HasKey(row => row.Id);
        builder.Property(row => row.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(row => row.SnapshotId).HasColumnName("snapshot_id");
        builder.Property(row => row.SeatRootSlotId).HasColumnName("seat_root_slot_id");
        builder.Property(row => row.EffectiveStart).HasColumnName("effective_start").HasColumnType("date");
        builder.Property(row => row.EffectiveEnd).HasColumnName("effective_end").HasColumnType("date");
        builder.Property(row => row.DailyValue).HasColumnName("daily_value").HasPrecision(18, 2);
        builder.Property(row => row.ExtraWorkingDayFees).HasColumnName("extra_working_day_fees").HasPrecision(18, 2);
        builder.Property(row => row.OverConsumption).HasColumnName("over_consumption").HasPrecision(18, 2);
        builder.Property(row => row.OverrideReason).HasColumnName("override_reason").HasMaxLength(500);
        builder.Property(row => row.AddedBy).HasColumnName("added_by");
        builder.Property(row => row.CreatedAt).HasColumnName("created_at");
        builder.Property(row => row.UpdatedAt).HasColumnName("updated_at");
        builder.Property(row => row.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(row => new { row.SnapshotId, row.SeatRootSlotId }).IsUnique()
            .HasFilter("\"deleted_at\" IS NULL");

        builder.HasOne(row => row.Snapshot)
            .WithMany(snapshot => snapshot.CommercialOverrides)
            .HasForeignKey(row => row.SnapshotId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(row => row.SeatRootSlot)
            .WithMany()
            .HasForeignKey(row => row.SeatRootSlotId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
