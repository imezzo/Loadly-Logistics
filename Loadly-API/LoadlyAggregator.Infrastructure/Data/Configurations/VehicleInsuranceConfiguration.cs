using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class VehicleInsuranceConfiguration : IEntityTypeConfiguration<VehicleInsurance>
{
    public void Configure(EntityTypeBuilder<VehicleInsurance> builder)
    {
        builder.ToTable("vehicles_insurance");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(v => v.PolicyNo)
            .HasColumnName("policy_no")
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(v => v.InsuranceProvider)
            .HasColumnName("insurance_provider")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(v => v.StartDate)
            .HasColumnName("start_date")
            .IsRequired();
        builder.Property(v => v.EndDate)
            .HasColumnName("end_date")
            .IsRequired();
        builder.Property(v => v.Amount)
            .HasColumnName("amount")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Property(v => v.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(UserStatus.ACTIVE)
            .HasSentinel((UserStatus)0);
        builder.Property(v => v.AddedBy)
            .HasColumnName("added_by");
        builder.Property(v => v.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(v => v.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(v => v.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.HasIndex(v => v.Status);
        builder.HasIndex(v => v.PolicyNo);
        builder.HasIndex(v => v.StartDate);
        builder.HasIndex(v => v.EndDate);
    }
}
