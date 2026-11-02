using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class CompanyDetailConfiguration : IEntityTypeConfiguration<CompanyDetail>
{
    public void Configure(EntityTypeBuilder<CompanyDetail> builder)
    {
        builder.ToTable("company_details");
        builder.HasKey(cd => cd.Id);
        builder.Property(cd => cd.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(cd => cd.CompanyId)
            .HasColumnName("company_id")
            .IsRequired();
        builder.Property(cd => cd.NameEn)
            .HasColumnName("name_en")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(cd => cd.NameAr)
            .HasColumnName("name_ar")
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(cd => cd.RegistrationNumber)
            .HasColumnName("registration_number")
            .HasMaxLength(100);
        builder.Property(cd => cd.TaxId)
            .HasColumnName("tax_id")
            .HasMaxLength(100);
        builder.Property(cd => cd.WebsiteUrl)
            .HasColumnName("website_url")
            .HasMaxLength(500);
        builder.Property(cd => cd.IndustryTypeId)
            .HasColumnName("industry_type_id");
        builder.Property(cd => cd.CompanySize)
            .HasColumnName("company_size")
            .HasMaxLength(100);
        builder.Property(cd => cd.ServiceRequirementsIds)
            .HasColumnName("service_requirements_ids")
            .HasColumnType("text");
        builder.Property(cd => cd.BusinessHourId)
            .HasColumnName("business_hour_id");
        builder.Property(cd => cd.CommunicationsMethodId)
            .HasColumnName("communications_method_id");
        builder.Property(cd => cd.VerificationStatus)
            .HasColumnName("verification_status")
            .HasConversion<int>()
            .HasDefaultValue(VerificationStatus.PENDING);
        builder.Property(cd => cd.QrCode)
            .HasColumnName("qr_code")
            .HasMaxLength(500);
        builder.Property(cd => cd.AddedBy)
            .HasColumnName("added_by");
        builder.Property(cd => cd.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(cd => cd.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(cd => cd.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.HasIndex(cd => cd.CompanyId).IsUnique();
        builder.HasIndex(cd => cd.VerificationStatus);
        builder.HasIndex(cd => cd.NameAr).IsUnique();
        builder.HasIndex(cd => cd.RegistrationNumber)
            .IsUnique()
            .HasFilter("\"registration_number\" IS NOT NULL");
        builder.HasIndex(cd => cd.WebsiteUrl).IsUnique();
        builder.HasOne(cd => cd.Company)
            .WithOne(c => c.CompanyDetail)
            .HasForeignKey<CompanyDetail>(cd => cd.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
