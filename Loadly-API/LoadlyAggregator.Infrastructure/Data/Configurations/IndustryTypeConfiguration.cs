using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class IndustryTypeConfiguration : IEntityTypeConfiguration<IndustryType>
{
    public void Configure(EntityTypeBuilder<IndustryType> builder)
    {
        builder.ToTable("industry_types");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(i => i.NameEn)
            .HasColumnName("name_en")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(i => i.NameAr)
            .HasColumnName("name_ar")
            .HasMaxLength(255);
        builder.Property(i => i.AddedBy)
            .HasColumnName("added_by");
        builder.Property(i => i.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(i => i.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(i => i.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
    }
}
