using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class DriverDetailConfiguration : IEntityTypeConfiguration<DriverDetail>
{
    public void Configure(EntityTypeBuilder<DriverDetail> builder)
    {
        builder.ToTable("driver_details");
        builder.HasKey(dd => dd.Id);
        builder.Property(dd => dd.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(dd => dd.DriverId)
            .HasColumnName("driver_id")
            .IsRequired();
        builder.Property(dd => dd.DateOfBirth)
            .HasColumnName("date_of_birth")
            .IsRequired(false);
        builder.Property(dd => dd.Gender)
            .HasColumnName("gender")
            .HasMaxLength(50)
            .IsRequired(false);
        builder.Property(dd => dd.LicenseTypeId)
            .HasColumnName("license_type_id")
            .IsRequired(false);
        builder.Property(dd => dd.LicenseIssueDate)
            .HasColumnName("license_issue_date")
            .IsRequired(false);
        builder.Property(dd => dd.LicenseNo)
            .HasColumnName("license_no")
            .HasMaxLength(100)
            .IsRequired(false);
        builder.Property(d => d.EmergencyContactName)
            .HasColumnName("emergency_contact_name")
            .IsRequired(false)
            .HasMaxLength(255);

        builder.Property(d => d.EmergencyContactPhone)
            .HasColumnName("emergency_contact_phone")
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(d => d.LicenseCategory)
            .HasColumnName("license_category")
            .HasConversion<int>()
            .HasDefaultValue(LicenseCategory.PrivateLicense);

        builder.Property(d => d.SpokenLanguages)
            .HasColumnName("spoken_languages")
            .HasColumnType("integer[]")
            .HasConversion(
                  v => v == null ? null : v.Select(x => (int)x).ToArray(),
                  v => v == null ? null : v.Select(x => (SpokenLanguage)x).ToList())
            .IsRequired(false)
            .Metadata.SetValueComparer(new ValueComparer<List<SpokenLanguage>?>(
                (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
                c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, (int)v)),
                c => c == null ? null : c.ToList()));

        builder.Property(dd => dd.LicenseExpirationDate)
            .HasColumnName("license_expiration_date")
            .IsRequired(false);
        builder.Property(dd => dd.VehicleTypeIds)
            .HasColumnName("vehicle_type_ids")
            .HasColumnType("text")
            .IsRequired(false);
        builder.Property(dd => dd.Age)
            .HasColumnName("age")
            .IsRequired(false);
        builder.Property(dd => dd.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(dd => dd.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(dd => dd.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.HasIndex(dd => dd.DriverId)
            .IsUnique();

        builder.HasIndex(dd => dd.LicenseTypeId);
        builder.HasOne(dd => dd.Driver)
            .WithOne(d => d.DriverDetail)
            .HasForeignKey<DriverDetail>(dd => dd.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<DriverLicenseType>()
            .WithMany()
            .HasForeignKey(dd => dd.LicenseTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
