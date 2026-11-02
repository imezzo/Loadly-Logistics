namespace LoadlyAggregator.Domain.Admin.DTOs.Contracts;

public sealed class ContractSlotChargesExcelFileDto
{
    public required byte[] Content { get; init; }
    public required string FileName { get; init; }
}

public sealed class ExportContractSlotChargesExcelDto
{
    public long ContractId { get; init; }
    public long? ContractSlotId { get; init; }
    public DateOnly PeriodStart { get; init; }
    public DateOnly PeriodEnd { get; init; }
}
