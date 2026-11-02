using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class FuelTypeConfiguration : IEntityTypeConfiguration<FuelType>
{
    public void Configure(EntityTypeBuilder<FuelType> builder)
    {
        builder.ToTable("fuel_types");
        builder.HasKey(ft => ft.Id);
        builder.Property(ft => ft.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(ft => ft.NameAr)
            .HasColumnName("name_ar")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(ft => ft.NameEn)
            .HasColumnName("name_en")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(ft => ft.UnitPrice)
            .HasColumnName("unit_price")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Property(ft => ft.AddedBy)
            .HasColumnName("added_by");
        builder.Property(ft => ft.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(ft => ft.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(ft => ft.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(ft => ft.NameAr).IsUnique();
        builder.HasIndex(ft => ft.NameEn).IsUnique();
    }
}
