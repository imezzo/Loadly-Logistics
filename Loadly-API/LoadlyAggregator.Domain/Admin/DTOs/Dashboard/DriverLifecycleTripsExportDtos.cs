namespace LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

public sealed class DriverLifecycleTripsFileDto
{
    public required byte[] Content { get; init; }
    public required string FileName { get; init; }
}

public sealed class ExportDriverLifecycleTripsDto
{
    public long? DriverId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public List<string>? Columns { get; init; }
}
