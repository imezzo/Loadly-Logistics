using LoadlyAggregator.Domain.Admin.DTOs.Contracts;

namespace LoadlyAggregator.API.Resources.Admin.Contracts;

public sealed class ContractDestinationsExcelFileResource
{
    public required byte[] Content { get; init; }
    public required string FileName { get; init; }

    public static ContractDestinationsExcelFileResource FromDto(ContractDestinationsExcelFileDto dto) =>
        new() { Content = dto.Content, FileName = dto.FileName };
}
