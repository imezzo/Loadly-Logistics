using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class CommunicationMethodConfiguration : IEntityTypeConfiguration<CommunicationMethod>
{
    public void Configure(EntityTypeBuilder<CommunicationMethod> builder)
    {
        builder.ToTable("communication_methods");
        builder.HasKey(cm => cm.Id);
        builder.Property(cm => cm.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(cm => cm.NameEn)
            .HasColumnName("name_en")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(cm => cm.NameAr)
            .HasColumnName("name_ar")
            .HasMaxLength(255);
        builder.Property(cm => cm.AddedBy)
            .HasColumnName("added_by");
        builder.Property(cm => cm.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(cm => cm.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(cm => cm.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
    }
}
