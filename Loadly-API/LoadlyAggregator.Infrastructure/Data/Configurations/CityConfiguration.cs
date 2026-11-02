using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("cities");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(c => c.NameEn)
            .HasColumnName("name_en")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(c => c.NameAr)
            .HasColumnName("name_ar")
            .HasMaxLength(255);
        builder.Property(c => c.GovernerateId)
            .HasColumnName("governerate_id")
            .IsRequired();
        builder.Property(c => c.AddedBy)
            .HasColumnName("added_by");
        builder.Property(c => c.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.HasIndex(c => c.GovernerateId);

        builder.HasOne(c => c.Governerate)
            .WithMany()
            .HasForeignKey(c => c.GovernerateId);
    }
}
