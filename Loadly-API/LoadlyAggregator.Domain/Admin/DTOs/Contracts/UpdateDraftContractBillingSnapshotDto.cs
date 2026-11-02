namespace LoadlyAggregator.Domain.Admin.DTOs.Contracts;

public class UpdateDraftContractBillingSnapshotDto
{
    public long ContractId { get; set; }
    public long SnapshotId { get; set; }
    public long? ContractSlotId { get; set; }
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public string? Name { get; set; }
}
