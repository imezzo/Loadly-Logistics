using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Resources.Admin.Vehicles;

public sealed class VehiclesExcelFileResource
{
    public required byte[] Content { get; init; }
    public required string FileName { get; init; }

    public static VehiclesExcelFileResource FromDto(VehiclesExcelFileDto dto) =>
        new() { Content = dto.Content, FileName = dto.FileName };
}
