using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class DistrictConfiguration : IEntityTypeConfiguration<District>
{
    public void Configure(EntityTypeBuilder<District> builder)
    {
        builder.ToTable("districts");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(d => d.NameEn)
            .HasColumnName("name_en")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(d => d.NameAr)
            .HasColumnName("name_ar")
            .HasMaxLength(255);
        builder.Property(d => d.CityId)
            .HasColumnName("city_id")
            .IsRequired();
        builder.Property(d => d.AddedBy)
            .HasColumnName("added_by");
        builder.Property(d => d.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(d => d.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.HasIndex(d => d.CityId);

        builder.HasOne(d => d.City)
            .WithMany()
            .HasForeignKey(d => d.CityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
