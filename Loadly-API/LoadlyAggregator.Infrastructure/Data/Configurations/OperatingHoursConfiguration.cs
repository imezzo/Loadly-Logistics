using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class OperatingHoursConfiguration : IEntityTypeConfiguration<OperatingHour>
{
    public void Configure(EntityTypeBuilder<OperatingHour> builder)
    {
        builder.ToTable("operating_hours");
        builder.HasKey(oh => oh.Id);

        builder.Property(oh => oh.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(oh => oh.NameEn)
            .HasColumnName("name_en")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(oh => oh.NameAr)
            .HasColumnName("name_ar")
            .HasMaxLength(255);

        builder.Property(oh => oh.AddedBy)
            .HasColumnName("added_by");

        builder.Property(oh => oh.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.Property(oh => oh.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(oh => oh.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
    }
}
