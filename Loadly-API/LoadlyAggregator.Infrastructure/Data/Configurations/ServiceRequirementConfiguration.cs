using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ServiceRequirementConfiguration : IEntityTypeConfiguration<ServiceRequirement>
{
    public void Configure(EntityTypeBuilder<ServiceRequirement> builder)
    {
        builder.ToTable("service_requirements");
        builder.HasKey(sr => sr.Id);
        builder.Property(sr => sr.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(sr => sr.NameEn)
            .HasColumnName("name_en")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(sr => sr.NameAr)
            .HasColumnName("name_ar")
            .HasMaxLength(255);
        builder.Property(sr => sr.AddedBy)
            .HasColumnName("added_by");
        builder.Property(sr => sr.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(sr => sr.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(sr => sr.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
    }
}
