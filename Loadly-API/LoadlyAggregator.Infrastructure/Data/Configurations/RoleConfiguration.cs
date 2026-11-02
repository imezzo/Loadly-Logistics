using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");
        builder.Property(r => r.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(r => r.NameAr)
            .HasColumnName("name_ar")
            .HasMaxLength(255)
            .IsRequired(false);
        builder.Property(r => r.GuardName)
            .HasColumnName("guard_name")
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(r => r.Description)
               .HasColumnName("description")
               .HasMaxLength(1000)
               .IsRequired(false);
        builder.Property(r => r.CompanyId)
            .HasColumnName("company_id")
            .IsRequired(false);
        builder.Property(r => r.DeletedAt)
            .HasColumnName("deleted_at");
        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at");
        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at");
        builder.HasIndex(r => r.GuardName);
        builder.HasIndex(r => r.Name);

        builder.HasIndex(r => new { r.Name, r.GuardName })
            .IsUnique()
            .HasFilter("\"company_id\" IS NULL");

        builder.HasIndex(r => new { r.Name, r.GuardName, r.CompanyId })
            .IsUnique()
            .HasFilter("\"company_id\" IS NOT NULL");

        builder.HasOne(r => r.Company)
            .WithMany()
            .HasForeignKey(r => r.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
