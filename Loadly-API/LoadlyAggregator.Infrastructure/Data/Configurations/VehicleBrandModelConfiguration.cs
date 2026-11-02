using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class VehicleBrandModelConfiguration : IEntityTypeConfiguration<VehicleBrandModel>
{
    public void Configure(EntityTypeBuilder<VehicleBrandModel> builder)
    {
        builder.ToTable("vehicle_brand_models");
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
        builder.Property(v => v.VehicleBrandId)
            .HasColumnName("vehicle_brand_id")
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

        builder.HasOne(v => v.VehicleBrand)
            .WithMany()
            .HasForeignKey(v => v.VehicleBrandId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(v => v.Status);
        builder.HasIndex(v => v.VehicleBrandId);
    }
}
