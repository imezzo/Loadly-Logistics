using LoadlyAggregator.Domain.Admin.DTOs.Trips;

namespace LoadlyAggregator.API.Resources.Admin.Trips;

public sealed class TripsExcelFileResource
{
    public required byte[] Content { get; init; }
    public required string FileName { get; init; }

    public static TripsExcelFileResource FromDto(TripsExcelFileDto dto) =>
        new() { Content = dto.Content, FileName = dto.FileName };
}
