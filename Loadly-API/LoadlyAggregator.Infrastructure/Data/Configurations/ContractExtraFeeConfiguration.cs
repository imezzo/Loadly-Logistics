using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ContractExtraFeeConfiguration : IEntityTypeConfiguration<ContractExtraFee>
{
    public void Configure(EntityTypeBuilder<ContractExtraFee> builder)
    {
        builder.ToTable("contract_extra_fees");
        builder.HasKey(cef => cef.Id);
        builder.Property(cef => cef.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(cef => cef.ContractId)
            .HasColumnName("contract_id")
            .IsRequired();
        builder.Property(cef => cef.ExtraFeeTypeId)
            .HasColumnName("extra_fee_type_id")
            .IsRequired();
        builder.Property(cef => cef.CalculationOption)
            .HasColumnName("calculation_option")
            .IsRequired(false);
        builder.Property(cef => cef.Value)
            .HasColumnName("value")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(cef => cef.Threshold)
            .HasColumnName("threshold")
            .IsRequired(false);
        builder.Property(cef => cef.Notes)
            .HasColumnName("notes")
            .HasColumnType("text")
            .IsRequired(false);
        builder.Property(cef => cef.AddedBy)
            .HasColumnName("added_by")
            .IsRequired(false);
        builder.Property(cef => cef.AllowOverride)
            .HasColumnName("allow_override")
            .IsRequired(false);
        builder.Property(cef => cef.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(cef => cef.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(cef => cef.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasOne(cef => cef.Contract)
            .WithMany(c => c.ContractExtraFees)
            .HasForeignKey(cef => cef.ContractId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(cef => cef.ContractId);
        builder.HasIndex(cef => cef.ExtraFeeTypeId);

        builder.HasIndex(cef => new { cef.ContractId, cef.ExtraFeeTypeId })
            .IsUnique()
            .HasDatabaseName("ix_contract_extra_fees_unique_combination")
            .HasFilter("\"deleted_at\" IS NULL");
    }
}
