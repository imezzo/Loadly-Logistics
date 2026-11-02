using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Infrastructure.Data.Configurations;

public class ContractApprovalConfiguration : IEntityTypeConfiguration<ContractApproval>
{
    public void Configure(EntityTypeBuilder<ContractApproval> builder)
    {
        builder.ToTable("contract_approvals");
        builder.HasKey(ca => ca.Id);
        builder.Property(ca => ca.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(ca => ca.ContractId)
            .HasColumnName("contract_id")
            .IsRequired();
        builder.Property(ca => ca.Stage)
            .HasColumnName("stage")
            .HasConversion<int>()
            .IsRequired();
        builder.Property(ca => ca.ApproverId)
            .HasColumnName("approver_id")
            .IsRequired(false);
        builder.Property(ca => ca.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired()
            .HasDefaultValue(ApprovalStatus.PENDING);
        builder.Property(ca => ca.Comments)
            .HasColumnName("comments")
            .HasColumnType("text")
            .IsRequired(false);
        builder.Property(ca => ca.ActionDate)
            .HasColumnName("action_date")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
        builder.Property(ca => ca.StageOrder)
            .HasColumnName("stage_order")
            .IsRequired();
        builder.Property(ca => ca.RowVersion)
            .HasColumnName("row_version")
            .IsRowVersion()
            .IsRequired();
        builder.Property(ca => ca.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(ca => ca.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.Property(ca => ca.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(ca => ca.ContractId);
        builder.HasIndex(ca => ca.ApproverId);
        builder.HasIndex(ca => ca.Status);
        builder.HasIndex(ca => new { ca.ContractId, ca.Stage }).IsUnique();

        builder.HasOne(ca => ca.Contract)
            .WithMany(c => c.Approvals)
            .HasForeignKey(ca => ca.ContractId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(ca => ca.Approver)
            .WithMany()
            .HasForeignKey(ca => ca.ApproverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
