namespace LoadlyAggregator.Domain.Admin.DTOs.Contracts;

public sealed class ContractBillingSnapshotExcelFileDto
{
    public required byte[] Content { get; init; }
    public required string FileName { get; init; }
}

public sealed class ExportContractBillingSnapshotExcelDto
{
    public long SnapshotId { get; init; }
}
