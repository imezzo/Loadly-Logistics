using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class CollectorConfiguration : IEntityTypeConfiguration<Collector>
{
    public void Configure(EntityTypeBuilder<Collector> builder)
    {
        builder.ToTable("collectors");

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
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Email)
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(c => c.Phone)
            .HasColumnName("phone")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Password)
            .HasColumnName("password")
            .IsRequired()
            .HasMaxLength(255);

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

        builder.Property(c => c.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasIndex(c => c.UserName).IsUnique();
        builder.HasIndex(c => c.Email);
        builder.HasIndex(c => c.Phone).IsUnique();
        builder.HasIndex(c => c.Status);

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
            .HasForeignKey(c => c.ModelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
