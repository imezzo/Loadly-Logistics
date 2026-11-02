using LoadlyAggregator.Domain.Admin.DTOs.Contracts;

namespace LoadlyAggregator.API.Resources.Admin.Contracts;

public class DraftContractBillingSnapshotResource
{
    public long SnapshotId { get; set; }
    public string Name { get; set; } = string.Empty;
    public CalculateIncludedContractSlotChargesResource Calculation { get; set; } = new();

    public static DraftContractBillingSnapshotResource FromResult(
        long snapshotId,
        string name,
        CalculateIncludedContractSlotChargesResultDto calculation) =>
        new()
        {
            SnapshotId = snapshotId,
            Name = name,
            Calculation = CalculateIncludedContractSlotChargesResource.FromDto(calculation)
        };
}
