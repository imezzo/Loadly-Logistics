using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ContractStatusHistoryConfiguration : IEntityTypeConfiguration<ContractStatusHistory>
{
    public void Configure(EntityTypeBuilder<ContractStatusHistory> builder)
    {
        builder.ToTable("contract_status_histories");
        builder.HasKey(csh => csh.Id);
        builder.Property(csh => csh.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(csh => csh.ContractId)
            .HasColumnName("contract_id")
            .IsRequired();
        builder.Property(csh => csh.PreviousStatus)
            .HasColumnName("previous_status")
            .HasConversion<int>()
            .IsRequired();
        builder.Property(csh => csh.NewStatus)
            .HasColumnName("new_status")
            .HasConversion<int>()
            .IsRequired();
        builder.Property(csh => csh.ApprovalStage)
            .HasColumnName("approval_stage")
            .HasConversion<int>()
            .IsRequired(false);
        builder.Property(csh => csh.ChangedBy)
            .HasColumnName("changed_by")
            .IsRequired(false);
        builder.Property(csh => csh.ChangedAt)
            .HasColumnName("changed_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        builder.Property(csh => csh.AdditionalNotes)
            .HasColumnName("additional_notes")
            .HasColumnType("text")
            .IsRequired(false);
        builder.Property(csh => csh.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(csh => csh.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(csh => csh.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(csh => csh.ContractId);
        builder.HasIndex(csh => csh.ChangedBy);
        builder.HasIndex(csh => csh.ChangedAt);

        builder.HasOne(csh => csh.Contract)
            .WithMany(c => c.StatusHistories)
            .HasForeignKey(csh => csh.ContractId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(csh => csh.ChangedByAdmin)
            .WithMany()
            .HasForeignKey(csh => csh.ChangedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
