using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("addresses");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(a => a.ModelType)
            .HasColumnName("model_type")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(a => a.ModelId)
            .HasColumnName("model_id")
            .IsRequired();
        builder.Property(a => a.StreetName)
            .HasColumnName("street_name")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(a => a.BuildingNumber)
            .HasColumnName("building_number")
            .HasMaxLength(100);
        builder.Property(a => a.BuildingType)
            .HasColumnName("building_type")
            .HasMaxLength(100);
        builder.Property(a => a.GovernerateId)
            .HasColumnName("governerate_id")
            .IsRequired();
        builder.Property(a => a.CityId)
            .HasColumnName("city_id")
            .IsRequired();
        builder.Property(a => a.DistrictId)
            .HasColumnName("district_id");
        builder.Property(a => a.PostalCode)
            .HasColumnName("postal_code")
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(a => a.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(AddressStatus.ACTIVE)
            .HasSentinel(AddressStatus.INACTIVE);
        builder.Property(a => a.AddedBy)
            .HasColumnName("added_by");
        builder.Property(a => a.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.HasIndex(a => new { a.ModelId, a.ModelType });
        builder.HasIndex(a => a.Status);
    }
}
