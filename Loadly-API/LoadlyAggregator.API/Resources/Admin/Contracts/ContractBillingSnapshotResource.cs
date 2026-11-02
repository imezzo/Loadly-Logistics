using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.API.Resources.Admin.Contracts;

public class ContractBillingSnapshotResource
{
    public long SnapshotId { get; set; }
    public string Name { get; set; } = string.Empty;
    public long ContractId { get; set; }
    public string ContractName { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public long CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public int ConsumptionType { get; set; }
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public long? InvoiceId { get; set; }
    public DateTime? FinalizedAt { get; set; }
    public int EngineVersion { get; set; }
    public ContractBillingSnapshotInvoicingResource Invoicing { get; set; } = new();
    public CalculateIncludedContractSlotChargesResource Calculation { get; set; } = new();

    public static ContractBillingSnapshotResource FromDto(ContractBillingSnapshotDetailDto dto) =>
        new()
        {
            SnapshotId = dto.SnapshotId,
            Name = dto.Name,
            ContractId = dto.ContractId,
            ContractName = dto.ContractName,
            ReferenceNumber = dto.ReferenceNumber,
            CompanyId = dto.CompanyId,
            CompanyName = dto.CompanyName,
            ConsumptionType = (int)dto.ConsumptionType,
            PeriodStart = dto.PeriodStart,
            PeriodEnd = dto.PeriodEnd,
            Status = (int)dto.Status,
            StatusName = dto.Status.ToLocalizedDisplay(),
            InvoiceId = dto.InvoiceId,
            FinalizedAt = dto.FinalizedAt,
            EngineVersion = dto.EngineVersion,
            Invoicing = ContractBillingSnapshotInvoicingResource.FromDto(dto.Invoicing),
            Calculation = CalculateIncludedContractSlotChargesResource.FromDto(dto.Calculation)
        };
}
