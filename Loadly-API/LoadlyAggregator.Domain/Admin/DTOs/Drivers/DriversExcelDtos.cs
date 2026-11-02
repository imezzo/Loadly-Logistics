using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Drivers;

public sealed class DriversExcelFileDto
{
    public required byte[] Content { get; init; }
    public required string FileName { get; init; }
}

public sealed class ExportDriversExcelDto
{
    public string? Search { get; init; }
    public int? Status { get; init; }
    public List<long>? ContractorIds { get; init; }
    public bool? HasContractor { get; init; }
    public bool? IsAvailable { get; init; }
    public LicenseCategory? LicenseCategory { get; init; }
    public List<DriverWorkingStatus>? WorkingStatuses { get; init; }
    public DateTime? TripsStartDateFrom { get; init; }
    public DateTime? TripsEndDateTo { get; init; }
    public bool? OrderByCompletedTripsDescending { get; init; }
    public List<string>? Columns { get; init; }
}
