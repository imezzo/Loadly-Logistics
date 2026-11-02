using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class VehicleTypeTagConfiguration : IEntityTypeConfiguration<VehicleTypeTag>
{
    public void Configure(EntityTypeBuilder<VehicleTypeTag> builder)
    {
        builder.ToTable("vehicle_type_tags");
        builder.HasKey(vtt => vtt.Id);
        builder.Property(vtt => vtt.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(vtt => vtt.VehicleTypeId)
            .HasColumnName("vehicle_type_id")
            .IsRequired();
        builder.Property(vtt => vtt.TagId)
            .HasColumnName("tag_id")
            .IsRequired();
        builder.Property(vtt => vtt.AddedBy)
            .HasColumnName("added_by");
        builder.Property(vtt => vtt.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(vtt => vtt.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(vtt => vtt.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(vtt => vtt.VehicleTypeId);
        builder.HasIndex(vtt => vtt.TagId);
        builder.HasIndex(vtt => new { vtt.VehicleTypeId, vtt.TagId })
            .IsUnique()
            .HasDatabaseName("ix_vehicle_type_tags_vehicle_type_id_tag_id")
            .HasFilter("\"deleted_at\" IS NULL");

        builder.HasOne(vtt => vtt.VehicleType)
            .WithMany(vt => vt.VehicleTypeTags)
            .HasForeignKey(vtt => vtt.VehicleTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(vtt => vtt.Tag)
            .WithMany(t => t.VehicleTypeTags)
            .HasForeignKey(vtt => vtt.TagId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
