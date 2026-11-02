using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ModelHasRoleConfiguration : IEntityTypeConfiguration<ModelHasRole>
{
    public void Configure(EntityTypeBuilder<ModelHasRole> builder)
    {
        builder.ToTable("model_has_roles");
        builder.HasKey(mhr => new { mhr.RoleId, mhr.ModelId, mhr.ModelType });
        builder.Property(mhr => mhr.RoleId)
            .HasColumnName("role_id")
            .HasColumnType("bigint");
        builder.Property(mhr => mhr.ModelId)
            .HasColumnName("model_id")
            .HasColumnType("bigint");
        builder.Property(mhr => mhr.ModelType)
            .HasColumnName("model_type")
            .IsRequired()
            .HasMaxLength(255);
        builder.HasOne(mhr => mhr.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(mhr => mhr.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(mhr => new { mhr.ModelId, mhr.ModelType });


        builder.HasQueryFilter(mhr => mhr.Role.DeletedAt == null);
    }
}
