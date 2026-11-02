using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class TripBookingInvitationConfiguration : IEntityTypeConfiguration<TripBookingInvitation>
{
    public void Configure(EntityTypeBuilder<TripBookingInvitation> builder)
    {
        builder.ToTable("trip_booking_invitations");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(i => i.TripId)
            .HasColumnName("trip_id")
            .IsRequired();
        builder.Property(i => i.ContractorId)
            .HasColumnName("contractor_id")
            .IsRequired();
        builder.Property(i => i.TripBookingInvitationRoundId)
            .HasColumnName("trip_booking_invitation_round_id")
            .IsRequired(false);
        builder.Property(i => i.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(TripBookingInvitationStatus.Pending)
            .IsRequired();
        builder.Property(i => i.AddedBy)
            .HasColumnName("added_by")
            .IsRequired(false);
        builder.Property(i => i.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(i => i.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(i => i.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasIndex(i => new { i.TripBookingInvitationRoundId, i.ContractorId })
            .IsUnique()
            .HasFilter("\"deleted_at\" IS NULL AND \"trip_booking_invitation_round_id\" IS NOT NULL");

        builder.HasOne(i => i.Trip)
            .WithMany(t => t.BookingInvitations)
            .HasForeignKey(i => i.TripId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Contractor)
            .WithMany()
            .HasForeignKey(i => i.ContractorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.TripBookingInvitationRound)
            .WithMany(r => r.BookingInvitations)
            .HasForeignKey(i => i.TripBookingInvitationRoundId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => i.ContractorId);
        builder.HasIndex(i => i.TripId);
        builder.HasIndex(i => i.TripBookingInvitationRoundId);
        builder.HasIndex(i => i.Status);
    }
}
