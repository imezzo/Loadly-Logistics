using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;

namespace LoadlyAggregator.Domain.Admin.DTOs.Contracts;

public class ContractBillingSnapshotDetailDto
{
    public long SnapshotId { get; set; }
    public string Name { get; set; } = string.Empty;
    public long ContractId { get; set; }
    public string ContractName { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public long CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public ContractConsumptionTypes ConsumptionType { get; set; }
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public ContractBillingSnapshotStatus Status { get; set; }
    public long? InvoiceId { get; set; }
    public DateTime? FinalizedAt { get; set; }
    public ContractBillingSnapshotInvoicingDto Invoicing { get; set; } = new();
    public int EngineVersion { get; set; }
    public CalculateIncludedContractSlotChargesResultDto Calculation { get; set; } = new();

    public static ContractBillingSnapshotDetailDto FromEntity(ContractBillingSnapshot snapshot) =>
        new()
        {
            SnapshotId = snapshot.Id,
            Name = snapshot.Name,
            ContractId = snapshot.ContractId,
            PeriodStart = snapshot.PeriodStart,
            PeriodEnd = snapshot.PeriodEnd,
            Status = snapshot.Status,
            InvoiceId = snapshot.InvoiceId,
            FinalizedAt = snapshot.FinalizedAt,
            EngineVersion = snapshot.EngineVersion,
            Calculation = ContractBillingSnapshotReader.ToCalculationDto(snapshot),
            Invoicing = ContractBillingSnapshotInvoicingDto.FromSnapshot(snapshot)
        };
}
