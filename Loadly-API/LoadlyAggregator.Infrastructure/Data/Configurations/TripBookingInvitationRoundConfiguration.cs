using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class TripBookingInvitationRoundConfiguration : IEntityTypeConfiguration<TripBookingInvitationRound>
{
    public void Configure(EntityTypeBuilder<TripBookingInvitationRound> builder)
    {
        builder.ToTable("trip_booking_invitation_rounds");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(r => r.TripId)
            .HasColumnName("trip_id")
            .IsRequired();
        builder.Property(r => r.ContractorId)
            .HasColumnName("contractor_id")
            .IsRequired(false);
        builder.Property(r => r.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(TripBookingInvitationRoundStatus.Active)
            .IsRequired();
        builder.Property(r => r.AddedBy)
            .HasColumnName("added_by")
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

        builder.HasOne(r => r.Trip)
            .WithMany(t => t.BookingInvitationRounds)
            .HasForeignKey(r => r.TripId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Contractor)
            .WithMany()
            .HasForeignKey(r => r.ContractorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => r.TripId);
        builder.HasIndex(r => r.ContractorId);
        builder.HasIndex(r => r.Status);
    }
}
