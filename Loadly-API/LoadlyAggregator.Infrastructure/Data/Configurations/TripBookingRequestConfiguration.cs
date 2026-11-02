using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class TripBookingRequestConfiguration : IEntityTypeConfiguration<TripBookingRequest>
{
    public void Configure(EntityTypeBuilder<TripBookingRequest> builder)
    {
        builder.ToTable("trip_booking_requests");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(r => r.TripId)
            .HasColumnName("trip_id")
            .IsRequired();
        builder.Property(r => r.ContractorId)
            .HasColumnName("contractor_id")
            .IsRequired();
        builder.Property(r => r.TripBookingInvitationRoundId)
            .HasColumnName("trip_booking_invitation_round_id")
            .IsRequired(false);
        builder.Property(r => r.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(TripBookingRequestStatus.Pending)
            .IsRequired();
        builder.Property(r => r.ReviewedAt)
            .HasColumnName("reviewed_at")
            .IsRequired(false);
        builder.Property(r => r.ReviewedByAdminId)
            .HasColumnName("reviewed_by_admin_id")
            .IsRequired(false);
        builder.Property(r => r.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasIndex(r => new { r.TripBookingInvitationRoundId, r.ContractorId })
            .IsUnique()
            .HasFilter(
                "\"deleted_at\" IS NULL AND \"trip_booking_invitation_round_id\" IS NOT NULL AND \"status\" = 0");

        builder.HasOne(r => r.Trip)
            .WithMany(t => t.BookingRequests)
            .HasForeignKey(r => r.TripId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Contractor)
            .WithMany()
            .HasForeignKey(r => r.ContractorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ReviewedByAdmin)
            .WithMany()
            .HasForeignKey(r => r.ReviewedByAdminId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.TripBookingInvitationRound)
            .WithMany(r => r.BookingRequests)
            .HasForeignKey(r => r.TripBookingInvitationRoundId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => r.TripId);
        builder.HasIndex(r => r.ContractorId);
        builder.HasIndex(r => r.TripBookingInvitationRoundId);
        builder.HasIndex(r => r.Status);
    }
}
