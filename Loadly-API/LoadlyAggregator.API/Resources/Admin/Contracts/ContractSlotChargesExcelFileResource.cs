using LoadlyAggregator.Domain.Admin.DTOs.Contracts;

namespace LoadlyAggregator.API.Resources.Admin.Contracts;

public sealed class ContractSlotChargesExcelFileResource
{
    public required byte[] Content { get; init; }
    public required string FileName { get; init; }

    public static ContractSlotChargesExcelFileResource FromDto(ContractSlotChargesExcelFileDto dto) =>
        new() { Content = dto.Content, FileName = dto.FileName };
}
