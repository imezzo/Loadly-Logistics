namespace LoadlyAggregator.Domain.Admin.DTOs.Contracts;

public sealed class ContractDestinationsExcelFileDto
{
    public required byte[] Content { get; init; }
    public required string FileName { get; init; }
}

public sealed class ExportContractDestinationsExcelDto
{
    public long Id { get; init; }
    public List<string>? Columns { get; init; }
}
