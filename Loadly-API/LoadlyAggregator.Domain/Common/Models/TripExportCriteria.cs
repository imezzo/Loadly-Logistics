using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Models;

public sealed class TripExportCriteria
{
    public string? Search { get; init; }
    public long? ContractId { get; init; }
    public ContractType? ContractTypeId { get; init; }
    public string? ContractReferenceNumber { get; init; }
    public List<long>? DestinationIds { get; init; }
    public long? CompanyId { get; init; }
    public List<long>? MatchingCompanyIds { get; init; }
    public long? VehicleId { get; init; }
    public long? VehicleTypeId { get; init; }
    public List<long>? SectorIds { get; init; }
    public long? DriverId { get; init; }
    public IEnumerable<long>? LeadIds { get; init; }
    public bool? IsExternalTrip { get; init; }
    public bool? IsBackfill { get; init; }
    public List<long>? ContractorIds { get; init; }
    public bool? HasContractor { get; init; }
    public int? Status { get; init; }
    public IEnumerable<int>? Statuses { get; init; }
    public OperationalDriverStatus? CurrentDriverStatus { get; init; }
    public TripLifecycleActor? LifecycleActor { get; init; }
    public IEnumerable<TripDocumentStatus>? TripDocumentStatuses { get; init; }
    public int? OverNightCostBearer { get; init; }
    public int? DispatchingMode { get; init; }
    public DateTime? StartDateFrom { get; init; }
    public DateTime? StartDateTo { get; init; }
    public DateOnly? FinancialDateFrom { get; init; }
    public DateOnly? FinancialDateTo { get; init; }
}
