using LoadlyAggregator.Domain.Admin.DTOs.Contracts;

namespace LoadlyAggregator.API.Resources.Admin.Contracts;

public sealed class ContractBillingSnapshotExcelFileResource
{
    public required byte[] Content { get; init; }
    public required string FileName { get; init; }

    public static ContractBillingSnapshotExcelFileResource FromDto(ContractBillingSnapshotExcelFileDto dto) =>
        new() { Content = dto.Content, FileName = dto.FileName };
}
