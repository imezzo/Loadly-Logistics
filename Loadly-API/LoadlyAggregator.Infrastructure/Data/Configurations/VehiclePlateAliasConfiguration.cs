using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class VehiclePlateAliasConfiguration : IEntityTypeConfiguration<VehiclePlateAlias>
{
    public void Configure(EntityTypeBuilder<VehiclePlateAlias> builder)
    {
        builder.ToTable("vehicle_plate_aliases");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(a => a.VehicleId)
            .HasColumnName("vehicle_id")
            .IsRequired();
        builder.Property(a => a.PlateNo)
            .HasColumnName("plate_no")
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(a => a.PlateLetters)
            .HasColumnName("plate_letters")
            .HasMaxLength(10)
            .IsRequired();
        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(a => a.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        builder.Property(a => a.AddedBy)
            .HasColumnName("added_by")
            .IsRequired(false);

        builder.HasIndex(a => new { a.PlateNo, a.PlateLetters })
            .IsUnique();
        builder.HasIndex(a => a.VehicleId);

        builder.HasOne(a => a.Vehicle)
            .WithMany(v => v.PlateAliases)
            .HasForeignKey(a => a.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(a => a.AddedByAdmin)
            .WithMany()
            .HasForeignKey(a => a.AddedBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
