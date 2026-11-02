using LoadlyAggregator.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ContractorConfiguration : IEntityTypeConfiguration<Contractor>
{
    public void Configure(EntityTypeBuilder<Contractor> builder)
    {
        builder.ToTable("contractors");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.FullName)
            .HasColumnName("full_name")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.UserName)
            .HasColumnName("username")
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(c => c.Email)
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(c => c.Phone)
            .HasColumnName("phone")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.NationalId)
            .HasColumnName("national_id")
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(c => c.SecondPhoneNumber)
            .HasColumnName("second_phone_number")
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(c => c.Address)
            .HasColumnName("address")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(c => c.Password)
            .HasColumnName("password")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(c => c.MfaChannel)
            .HasColumnName("mfa_channel")
            .HasConversion<int>()
            .HasDefaultValue(CommunicationChannel.None);

        builder.Property(c => c.EmailVerified)
            .HasColumnName("email_verified")
            .HasDefaultValue(false);

        builder.Property(c => c.PhoneVerified)
            .HasColumnName("phone_verified")
            .HasDefaultValue(false);

        builder.Property(c => c.Status)
            .HasColumnName("status")
            .HasConversion<int>();

        builder.Property(c => c.ContractorStatus)
            .HasColumnName("contractor_status")
            .HasConversion<int>();

        builder.Property(c => c.ApprovedAt)
            .HasColumnName("approved_at")
            .IsRequired(false);

        builder.Property(c => c.ApprovedBy)
            .HasColumnName("approved_by")
            .IsRequired(false);

        builder.Property(c => c.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasIndex(c => c.Phone).IsUnique();
        builder.HasIndex(c => c.SecondPhoneNumber)
            .IsUnique()
            .HasFilter("\"second_phone_number\" IS NOT NULL AND \"second_phone_number\" <> ''");
        builder.HasIndex(c => c.NationalId)
            .IsUnique()
            .HasFilter("\"national_id\" IS NOT NULL AND \"national_id\" <> ''");
        builder.HasIndex(c => c.UserName)
            .IsUnique()
            .HasFilter("\"username\" IS NOT NULL AND \"username\" <> ''");
        builder.HasIndex(c => c.Email)
            .IsUnique()
            .HasFilter("\"email\" IS NOT NULL AND \"email\" <> ''");
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.ContractorStatus);

        builder.HasMany(c => c.UserRoles)
            .WithOne()
            .HasForeignKey(mhr => new { mhr.ModelId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.DirectPermissions)
            .WithOne()
            .HasForeignKey(mhp => new { mhp.ModelId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Addresses)
            .WithOne()
            .HasForeignKey(a => a.ModelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Contacts)
            .WithOne()
            .HasForeignKey(contact => contact.ModelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
