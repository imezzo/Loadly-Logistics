using LoadlyAggregator.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.ToTable("drivers");



        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(d => d.FullName)
            .HasColumnName("fullname_en")
            .IsRequired(false)
            .HasMaxLength(255);

        builder.Property(d => d.FullNameAr)
            .HasColumnName("fullname_ar")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.NationalId)
            .HasColumnName("national_id")
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(d => d.UserName)
            .HasColumnName("username")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Email)
            .HasColumnName("email")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.Phone)
            .HasColumnName("phone")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(d => d.Password)
            .HasColumnName("password")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.MfaChannel)
            .HasColumnName("mfa_channel")
            .HasConversion<int>()
            .HasDefaultValue(CommunicationChannel.None);

        builder.Property(d => d.EmailVerified)
            .HasColumnName("email_verified")
            .HasDefaultValue(false);

        builder.Property(d => d.PhoneVerified)
            .HasColumnName("phone_verified")
            .HasDefaultValue(false);

        builder.Property(d => d.Status)
            .HasColumnName("status")
            .HasConversion<int>();

        builder.Property(d => d.ProfilePicture)
            .HasColumnName("profile_picture")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(d => d.BiometricFingerprint)
            .HasColumnName("biometric_fingerprint")
            .HasDefaultValue(false);

        builder.Property(d => d.WorkingStatus)
            .HasColumnName("working_status")
            .HasConversion<int>()
            .HasDefaultValue(DriverWorkingStatus.Available);

        builder.Property(d => d.ContractorId)
            .HasColumnName("contractor_id")
            .IsRequired(false);

        builder.Property(d => d.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasIndex(d => d.UserName).IsUnique();
        builder.HasIndex(d => d.Email).IsUnique();
        builder.HasIndex(d => d.Phone).IsUnique();
        builder.HasIndex(d => d.Status);

        builder.HasMany(d => d.UserRoles)
            .WithOne()
            .HasForeignKey(mhr => new { mhr.ModelId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.DirectPermissions)
            .WithOne()
            .HasForeignKey(mhp => new { mhp.ModelId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.Addresses)
            .WithOne()
            .HasForeignKey(a => a.ModelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.Contacts)
            .WithOne()
            .HasForeignKey(c => c.ModelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Contractor)
            .WithMany(c => c.Drivers)
            .HasForeignKey(d => d.ContractorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(d => d.ContractorId);
    }
}

