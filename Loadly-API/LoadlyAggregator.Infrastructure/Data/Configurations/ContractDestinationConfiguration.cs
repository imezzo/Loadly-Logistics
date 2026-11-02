using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ContractDestinationConfiguration : IEntityTypeConfiguration<ContractDestination>
{
    public void Configure(EntityTypeBuilder<ContractDestination> builder)
    {
        builder.ToTable("contract_destinations");

        builder.HasKey(cd => cd.Id);
        builder.Property(cd => cd.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(cd => cd.ContractId)
            .HasColumnName("contract_id")
            .IsRequired();

        builder.Property(cd => cd.StartWaypointId)
            .HasColumnName("start_waypoint_id")
            .IsRequired();

        builder.Property(cd => cd.EndWaypointId)
            .HasColumnName("end_waypoint_id")
            .IsRequired();

        builder.Property(cd => cd.NameAr)
            .HasColumnName("name_ar")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(cd => cd.NameEn)
            .HasColumnName("name_en")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(cd => cd.ExpectedKms)
            .HasColumnName("expected_kms")
            .HasColumnType("numeric(10,2)")
            .IsRequired(false);

        builder.Property(cd => cd.ReturnExpectedKms)
            .HasColumnName("return_expected_kms")
            .HasColumnType("numeric(10,2)")
            .IsRequired(false);

        builder.Property(cd => cd.NumberOfDays)
            .HasColumnName("number_of_days")
            .IsRequired(false);

        builder.Property(cd => cd.PricePerTon)
            .HasColumnName("price_per_ton")
            .HasColumnType("numeric(10,2)")
            .IsRequired(false);

        builder.Property(cd => cd.FixedTripPrice)
            .HasColumnName("fixed_trip_price")
            .HasColumnType("numeric(10,2)")
            .IsRequired(false);

        builder.Property(cd => cd.ContractorPricePerTon)
            .HasColumnName("contractor_price_per_ton")
            .HasColumnType("numeric(10,2)")
            .IsRequired(false);

        builder.Property(cd => cd.ContractorFixedTripPrice)
            .HasColumnName("contractor_fixed_trip_price")
            .HasColumnType("numeric(10,2)")
            .IsRequired(false);

        builder.Property(cd => cd.Description)
            .HasColumnName("description")
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(cd => cd.Status)
            .HasColumnName("status")
            .HasConversion<int?>()
            .IsRequired(false)
            .HasDefaultValue(ContractDestinationStatus.ACTIVE);

        builder.Property(cd => cd.AddedBy)
            .HasColumnName("added_by")
            .IsRequired(false);

        builder.Property(cd => cd.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(cd => cd.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(cd => cd.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(cd => cd.ContractId);
        builder.HasIndex(cd => cd.StartWaypointId);
        builder.HasIndex(cd => cd.EndWaypointId);

        builder.HasIndex(cd => new { cd.ContractId, cd.StartWaypointId, cd.EndWaypointId })
            .IsUnique();

        builder.HasOne(cd => cd.Contract)
            .WithMany(c => c.ContractDestinations)
            .HasForeignKey(cd => cd.ContractId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cd => cd.StartWaypointNavigation)
            .WithMany()
            .HasForeignKey(cd => cd.StartWaypointId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cd => cd.EndWaypointNavigation)
            .WithMany()
            .HasForeignKey(cd => cd.EndWaypointId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
