using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class VehicleFacilityConfiguration : IEntityTypeConfiguration<VehicleFacility>
{
    public void Configure(EntityTypeBuilder<VehicleFacility> builder)
    {
        builder.ToTable("vehicle_facilities");

        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(v => v.Name)
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(v => v.Address)
            .HasColumnName("address")
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(v => v.GoogleMapsUrl)
            .HasColumnName("google_maps_url")
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(v => v.Lat)
            .HasColumnName("lat")
            .HasColumnType("decimal(10,8)")
            .IsRequired(false);

        builder.Property(v => v.Lng)
            .HasColumnName("lng")
            .HasColumnType("decimal(11,8)")
            .IsRequired(false);

        builder.Property(v => v.AddedBy)
            .HasColumnName("added_by")
            .IsRequired();

        builder.Property(v => v.CompanyId)
            .HasColumnName("company_id")
            .IsRequired(false);

        builder.Property(v => v.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(VehicleFacilityStatus.ACTIVE)
            .HasSentinel(VehicleFacilityStatus.INACTIVE);

        builder.Property(v => v.FacilityType)
            .HasColumnName("facility_type")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(v => v.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(v => v.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(v => v.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(v => v.Status);
        builder.HasIndex(v => v.FacilityType);
        builder.HasIndex(v => v.CompanyId);

        builder.HasOne(v => v.Company)
            .WithMany()
            .HasForeignKey(v => v.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(v => v.Contacts)
            .WithOne()
            .HasForeignKey(c => c.ModelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
