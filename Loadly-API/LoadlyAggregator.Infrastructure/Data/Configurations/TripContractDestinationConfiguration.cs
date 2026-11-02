using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class TripContractDestinationConfiguration : IEntityTypeConfiguration<TripContractDestination>
{
    public void Configure(EntityTypeBuilder<TripContractDestination> builder)
    {
        builder.ToTable("trip_contract_destinations");

        builder.HasKey(tcd => tcd.Id);
        builder.Property(tcd => tcd.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(tcd => tcd.TripId)
            .HasColumnName("trip_id")
            .IsRequired();

        builder.Property(tcd => tcd.DestinationId)
            .HasColumnName("destination_id")
            .IsRequired();

        builder.Property(tcd => tcd.ReturnDestinationId)
            .HasColumnName("return_destination_id")
            .IsRequired(false);

        builder.Property(tcd => tcd.OutboundDistanceKm)
            .HasColumnName("outbound_distance_km")
            .HasColumnType("numeric(10,2)")
            .IsRequired();

        builder.Property(tcd => tcd.ReturnDistanceKm)
            .HasColumnName("return_distance_km")
            .HasColumnType("numeric(10,2)")
            .IsRequired(false);

        builder.Property(tcd => tcd.AddedBy)
            .HasColumnName("added_by")
            .IsRequired();

        builder.Property(tcd => tcd.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(tcd => tcd.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(tcd => tcd.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(tcd => tcd.TripId)
            .IsUnique()
            .HasFilter("\"deleted_at\" IS NULL");
        builder.HasIndex(tcd => tcd.DestinationId);
        builder.HasIndex(tcd => tcd.ReturnDestinationId);
        builder.HasIndex(tcd => tcd.AddedBy);

        builder.HasOne(tcd => tcd.Trip)
            .WithOne(t => t.ContractDestinationAssignment)
            .HasForeignKey<TripContractDestination>(tcd => tcd.TripId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(tcd => tcd.Destination)
            .WithMany()
            .HasForeignKey(tcd => tcd.DestinationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(tcd => tcd.ReturnDestination)
            .WithMany()
            .HasForeignKey(tcd => tcd.ReturnDestinationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(tcd => tcd.AddedByAdmin)
            .WithMany()
            .HasForeignKey(tcd => tcd.AddedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
