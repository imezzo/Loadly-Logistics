using LoadlyAggregator.Domain.Admin.DTOs.Drivers;

namespace LoadlyAggregator.API.Resources.Admin.Drivers;

public sealed class DriversExcelFileResource
{
    public required byte[] Content { get; init; }
    public required string FileName { get; init; }

    public static DriversExcelFileResource FromDto(DriversExcelFileDto dto) =>
        new() { Content = dto.Content, FileName = dto.FileName };
}
