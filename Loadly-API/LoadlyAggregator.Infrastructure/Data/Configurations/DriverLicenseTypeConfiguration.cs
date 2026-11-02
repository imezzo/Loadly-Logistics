using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class DriverLicenseTypeConfiguration : IEntityTypeConfiguration<DriverLicenseType>
{
    public void Configure(EntityTypeBuilder<DriverLicenseType> builder)
    {
        builder.ToTable("driver_license_types");
        builder.HasKey(dlt => dlt.Id);
        builder.Property(dlt => dlt.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(dlt => dlt.NameEn)
            .HasColumnName("name_en")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(dlt => dlt.NameAr)
            .HasColumnName("name_ar")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(dlt => dlt.AddedBy)
            .HasColumnName("added_by");
        builder.Property(dlt => dlt.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(UserStatus.ACTIVE)
            .HasSentinel((UserStatus)0);
        builder.Property(dlt => dlt.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(dlt => dlt.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(dlt => dlt.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.HasIndex(dlt => dlt.Status);
    }
}
