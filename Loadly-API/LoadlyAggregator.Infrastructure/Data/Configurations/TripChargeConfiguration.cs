using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class TripChargeConfiguration : IEntityTypeConfiguration<TripCharge>
{
    public void Configure(EntityTypeBuilder<TripCharge> builder)
    {
        builder.ToTable("trip_charges");
        builder.HasKey(tc => tc.Id);
        builder.Property(tc => tc.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(tc => tc.TripId)
            .HasColumnName("trip_id")
            .IsRequired();
        builder.Property(tc => tc.ChargeType)
            .HasColumnName("charge_type")
            .HasConversion<int>()
            .IsRequired();
        builder.Property(tc => tc.Amount)
            .HasColumnName("amount")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Property(tc => tc.CompanyShare)
            .HasColumnName("company_share")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
        builder.Property(tc => tc.ExtraFeeTypeId)
            .HasColumnName("extra_fee_type_id")
            .IsRequired(false);
        builder.Property(tc => tc.InvoiceType)
            .HasColumnName("invoice_type")
            .HasConversion<int>()
            .IsRequired(false);
        builder.Property(tc => tc.Notes)
            .HasColumnName("notes")
            .HasColumnType("text")
            .IsRequired(false);
        builder.Property(tc => tc.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(tc => tc.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(tc => tc.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasOne(tc => tc.Trip)
            .WithMany(t => t.TripCharges)
            .HasForeignKey(tc => tc.TripId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(tc => tc.TripId);
    }
}
