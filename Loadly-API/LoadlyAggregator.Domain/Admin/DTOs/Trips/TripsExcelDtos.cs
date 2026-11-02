using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public sealed class TripsExcelFileDto
{
    public required byte[] Content { get; init; }
    public required string FileName { get; init; }
}

public sealed class ExportTripsExcelDto
{
    public string? Search { get; init; }
    public long? ContractId { get; init; }
    public ContractType? ContractTypeId { get; init; }
    public string? ContractReferenceNumber { get; init; }
    public List<long>? DestinationIds { get; init; }
    public long? CompanyId { get; init; }
    public long? VehicleId { get; init; }
    public long? VehicleTypeId { get; init; }
    public List<long>? SectorIds { get; init; }
    public long? DriverId { get; init; }
    public List<long>? LeadIds { get; init; }
    public bool? IsExternalTrip { get; init; }
    public bool? IsBackfill { get; init; }
    public List<long>? ContractorIds { get; init; }
    public bool? HasContractor { get; init; }
    public TripStatus? Status { get; init; }
    public OperationalDriverStatus? CurrentDriverStatus { get; init; }
    public TripLifecycleActor? LifecycleActor { get; init; }
    public List<TripDocumentStatus>? TripDocumentStatuses { get; init; }
    public OverNightCostBearer? OverNightCostBearer { get; init; }
    public TripDispatchingMode? DispatchingMode { get; init; }
    public DateTime? StartDateFrom { get; init; }
    public DateTime? StartDateTo { get; init; }
    public DateOnly? FinancialDateFrom { get; init; }
    public DateOnly? FinancialDateTo { get; init; }
    public List<string>? Columns { get; init; }
}
