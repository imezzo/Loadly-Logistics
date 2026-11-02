using LoadlyAggregator.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class AdminConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.ToTable("admins");



        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(a => a.Type)
            .HasColumnName("type")
            .HasConversion<int>()
            .HasDefaultValue(AdminType.ADMIN)
            .HasSentinel((AdminType)0);

        builder.Property(a => a.FullName)
            .HasColumnName("full_name")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.UserName)
            .HasColumnName("username")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Email)
            .HasColumnName("email")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.Phone)
            .HasColumnName("phone")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.Password)
            .HasColumnName("password")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.MfaChannel)
            .HasColumnName("mfa_channel")
            .HasConversion<int>()
            .HasDefaultValue(CommunicationChannel.None);

        builder.Property(a => a.EmailVerified)
            .HasColumnName("email_verified")
            .HasDefaultValue(false);

        builder.Property(a => a.PhoneVerified)
            .HasColumnName("phone_verified")
            .HasDefaultValue(false);

        builder.Property(a => a.Status)
            .HasColumnName("status")
            .HasConversion<int>();

        builder.Property(a => a.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasIndex(a => a.UserName).IsUnique();
        builder.HasIndex(a => a.Phone).IsUnique();
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.Type);

        builder.HasMany(a => a.UserRoles)
            .WithOne()
            .HasForeignKey(mhr => new { mhr.ModelId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.DirectPermissions)
            .WithOne()
            .HasForeignKey(mhp => new { mhp.ModelId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Addresses)
            .WithOne()
            .HasForeignKey(a => a.ModelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Contacts)
            .WithOne()
            .HasForeignKey(c => c.ModelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

