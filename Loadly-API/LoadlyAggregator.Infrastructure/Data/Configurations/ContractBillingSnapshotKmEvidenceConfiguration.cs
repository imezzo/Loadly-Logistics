using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ContractBillingSnapshotKmEvidenceConfiguration : IEntityTypeConfiguration<ContractBillingSnapshotKmEvidence>
{
    public void Configure(EntityTypeBuilder<ContractBillingSnapshotKmEvidence> builder)
    {
        builder.ToTable("contract_billing_snapshot_km_evidence");
        builder.HasKey(km => km.Id);
        builder.Property(km => km.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(km => km.SnapshotId).HasColumnName("snapshot_id");
        builder.Property(km => km.ContractSlotId).HasColumnName("contract_slot_id");
        builder.Property(km => km.IsPooled).HasColumnName("is_pooled");
        builder.Property(km => km.Consumption).HasColumnName("consumption").HasColumnType("decimal(18,2)");
        builder.Property(km => km.MaxKms).HasColumnName("max_kms").HasColumnType("decimal(18,2)");
        builder.Property(km => km.OverageKm).HasColumnName("overage_km").HasColumnType("decimal(18,2)");
        builder.Property(km => km.OverConsumptionRate).HasColumnName("over_consumption_rate").HasColumnType("decimal(18,2)");
        builder.Property(km => km.Amount).HasColumnName("amount").HasColumnType("decimal(18,2)");
        builder.Property(km => km.CreatedAt).HasColumnName("created_at");
        builder.Property(km => km.UpdatedAt).HasColumnName("updated_at");
        builder.Property(km => km.DeletedAt).HasColumnName("deleted_at");

        builder.HasOne(km => km.Snapshot)
            .WithMany(snapshot => snapshot.KmEvidence)
            .HasForeignKey(km => km.SnapshotId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
