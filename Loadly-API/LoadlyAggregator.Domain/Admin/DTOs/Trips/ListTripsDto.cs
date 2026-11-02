using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class ListTripsDto : BasePagedRequest
{
    public long? ContractId { get; set; }
    public ContractType? ContractTypeId { get; set; }
    public string? ContractReferenceNumber { get; set; }  // Filter by contract reference number
    public List<long>? DestinationIds { get; set; }
    public long? CompanyId { get; set; }  // Filter by company ID
    public long? VehicleId { get; set; }
    public long? VehicleTypeId { get; set; }
    public long? ContractVehicleTypeId { get; set; }
    public bool? IsCreatedByCompany { get; set; }
    public List<long>? SectorIds { get; set; }
    public long? DriverId { get; set; }  // Filter by driver ID
    public List<long>? LeadIds { get; set; }
    public bool? IsExternalTrip { get; set; }
    public bool? IsBackfill { get; set; }
    public List<long>? ContractorIds { get; set; }
    public bool? HasContractor { get; set; }
    public TripStatus? Status { get; set; }  // Filter by trip status
    public OperationalDriverStatus? CurrentDriverStatus { get; set; }
    public List<TripStatus>? Statuses { get; set; }  // Filter by multiple trip statuses
    public TripLifecycleActor? LifecycleActor { get; set; }
    public List<TripDocumentStatus>? TripDocumentStatuses { get; set; }
    public OverNightCostBearer? OverNightCostBearer { get; set; }
    public TripDispatchingMode? DispatchingMode { get; set; }
    public bool? IsPushedToContractors { get; set; }
    public bool? HasContractorInvoice { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateOnly? FinancialDateFrom { get; set; }
    public DateOnly? FinancialDateTo { get; set; }
}
