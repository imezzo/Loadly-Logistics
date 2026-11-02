using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("contacts");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(c => c.ModelType)
            .HasColumnName("model_type")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(c => c.ModelId)
            .HasColumnName("model_id")
            .IsRequired();
        builder.Property(c => c.Primary)
            .HasColumnName("primary")
            .HasDefaultValue(false);
        builder.Property(c => c.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(c => c.Email)
            .HasColumnName("email")
            .HasMaxLength(255);
        builder.Property(c => c.Phone)
            .HasColumnName("phone")
            .HasMaxLength(50);
        builder.Property(c => c.Position)
            .HasColumnName("position")
            .HasMaxLength(255);
        builder.Property(c => c.CommunicationMethodId)
            .HasColumnName("communication_method_id");
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
        builder.HasIndex(c => new { c.ModelId, c.ModelType });
        builder.HasIndex(c => c.Primary);
        builder.HasIndex(c => c.CommunicationMethodId);
        builder.HasOne(c => c.CommunicationMethod)
            .WithMany()
            .HasForeignKey(c => c.CommunicationMethodId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
