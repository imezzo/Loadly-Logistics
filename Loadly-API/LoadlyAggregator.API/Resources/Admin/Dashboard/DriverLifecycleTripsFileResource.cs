using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

namespace LoadlyAggregator.API.Resources.Admin.Dashboard;

public sealed class DriverLifecycleTripsFileResource
{
    public required byte[] Content { get; init; }
    public required string FileName { get; init; }

    public static DriverLifecycleTripsFileResource FromDto(DriverLifecycleTripsFileDto dto) =>
        new() { Content = dto.Content, FileName = dto.FileName };
}