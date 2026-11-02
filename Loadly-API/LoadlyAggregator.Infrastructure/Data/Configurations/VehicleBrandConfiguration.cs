using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class VehicleBrandConfiguration : IEntityTypeConfiguration<VehicleBrand>
{
    public void Configure(EntityTypeBuilder<VehicleBrand> builder)
    {
        builder.ToTable("vehicle_brands");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(v => v.NameAr)
            .HasColumnName("name_ar")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(v => v.NameEn)
            .HasColumnName("name_en")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(v => v.Logo)
            .HasColumnName("logo")
            .HasMaxLength(500)
            .IsRequired(false);
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
    }
}
