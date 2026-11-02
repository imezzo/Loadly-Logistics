using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ContractBillingSnapshotChargeLineConfiguration : IEntityTypeConfiguration<ContractBillingSnapshotChargeLine>
{
    public void Configure(EntityTypeBuilder<ContractBillingSnapshotChargeLine> builder)
    {
        builder.ToTable("contract_billing_snapshot_charge_lines");
        builder.HasKey(line => line.Id);
        builder.Property(line => line.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(line => line.SnapshotId).HasColumnName("snapshot_id");
        builder.Property(line => line.ContractSlotId).HasColumnName("contract_slot_id");
        builder.Property(line => line.Date).HasColumnName("date").HasColumnType("date");
        builder.Property(line => line.ChargeKind).HasColumnName("charge_kind").HasConversion<int>();
        builder.Property(line => line.Amount).HasColumnName("amount").HasColumnType("decimal(18,2)");
        builder.Property(line => line.CreatedAt).HasColumnName("created_at");
        builder.Property(line => line.UpdatedAt).HasColumnName("updated_at");
        builder.Property(line => line.DeletedAt).HasColumnName("deleted_at");

        builder.HasOne(line => line.Snapshot)
            .WithMany(snapshot => snapshot.ChargeLines)
            .HasForeignKey(line => line.SnapshotId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
