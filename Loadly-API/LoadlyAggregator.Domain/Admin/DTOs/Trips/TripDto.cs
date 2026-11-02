using LoadlyAggregator.Domain.Common.DTOs.Relations;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class TripDto
{
    public long Id { get; set; }
    public long? ContractId { get; set; }
    public long? ContractVehicleTypeId { get; set; }
    public long? VehicleTypeId { get; set; }
    public long? VehicleId { get; set; }
    public long? TailVehicleId { get; set; }
    public long? DriverId { get; set; }
    public long? LeadId { get; set; }
    public long? ContractorId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? CompletionTime { get; set; }
    public DateOnly? FinancialDate { get; set; }
    public TripStatus Status { get; set; }
    public string TripStatusName { get; set; } = string.Empty;
    public OperationalDriverStatus? CurrentDriverStatus { get; set; }
    public string CurrentDriverStatusName { get; set; } = string.Empty;
    public TripDocumentStatus TripDocumentStatus { get; set; }
    public TripCustomsStatus CustomsStatus { get; set; }
    public string CustomsStatusName { get; set; } = string.Empty;
    public decimal? CustomsTotalAmount { get; set; }
    public bool HasCustoms => CustomsStatus != TripCustomsStatus.None;
    public decimal? ContractDistanceKm { get; set; }
    public decimal? ActualDistanceKm { get; set; }
    public decimal? ActualCargoWeightTons { get; set; }
    public decimal? CargoPrice { get; set; }
    public decimal? SpotPrice { get; set; }
    public int? NumberOfDays { get; set; }
    public OverNightCostBearer? OverNightCostBearer { get; set; }
    public TripDispatchingMode? DispatchingMode { get; set; }
    public TripType? TripType { get; set; }
    public string TripTypeName { get; set; } = string.Empty;
    public decimal? TotalAddonsCost { get; set; }
    public decimal? TotalTripCost { get; set; }
    public decimal? ContractorPrice { get; set; }
    public string? CancellationReason { get; set; }
    public bool IsExternalTrip { get; set; } = false;
    public bool? IsBackfill { get; set; }
    public string? ContactPersonName { get; set; }
    public string? ContactPersonMobile { get; set; }
    public string? DispatcherNotes { get; set; }
    public long? DispatcherId { get; set; }
    public AdminRelationDto? Dispatcher { get; set; }
    public string? ProofOfDelivery { get; set; }
    public long? AuditedById { get; set; }
    public DateTime? AuditedAt { get; set; }
    public AdminRelationDto? AuditedBy { get; set; }
    public bool IsEditable { get; set; } = true;
    public ContractConditionsDocumentDto? ContractConditions { get; set; }
    public int? HelpersCount { get; set; }
    public int CutoffDuration { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Strongly-typed nested objects for complete information
    public DriverRelationDto? Driver { get; set; }
    public ContractorRelationDto? Contractor { get; set; }
    public ContractRelationDto? Contract { get; set; }
    public ContractVehicleTypeRelationDto? ContractVehicleType { get; set; }
    public List<TripContractDestinationDto>? TripContractDestinations { get; set; }
    public VehicleRelationDto? Vehicle { get; set; }
    public TripVehicleAssignmentSnapshotDto? VehicleSnapshot { get; set; }
    public bool IsHeadVehiclePlateChanged { get; set; }
    public bool IsTailVehiclePlateChanged { get; set; }
    public DateTime? HeadPlateChangedAt { get; set; }
    public DateTime? TailPlateChangedAt { get; set; }
    public CoupledVehicleRelationDto? TailVehicle { get; set; }
    public CompanyRelationDto? Company { get; set; }
    public LeadRelationDto? Lead { get; set; }
    public List<TripWaypointDto>? Waypoints { get; set; }
    public List<TripRecordingDto>? Recordings { get; set; }
    public List<TripDriverDocumentDto>? Documents { get; set; }
    public List<TripDocumentDto>? CollectorDocuments { get; set; }
    public List<TripChargeDto>? TripCharges { get; set; }
    public List<TripBookingRequestDto>? BookingRequests { get; set; }
    public TripLocationDto? CurrentLocation { get; set; }
    public TripLocationDto? StartLocation { get; set; }
    public InvoiceRelationDto? Invoice { get; set; }
    public InvoiceRelationDto? ClaimInvoice { get; set; }
    public TripInvoicingStatus InvoicingStatus { get; set; }
    public string InvoicingStatusName { get; set; } = string.Empty;
    public bool RequiresMainInvoice { get; set; }
    public bool RequiresClaimInvoice { get; set; }
    public bool FullyInvoiced { get; set; }
    public decimal TotalBillableAmount { get; set; }
    public decimal TotalBillableTripItemsAmount { get; set; }
    public decimal TotalBillableClaimItemsAmount { get; set; }
    public decimal TotalFuelAmount { get; set; }
    public decimal TotalBillableFuelAmount { get; set; }
    public FuelPricingCalculationOption? FuelPricingCalculationOption { get; set; }
    public decimal TotalBillableTollGatesAmount { get; set; }
    public TripLifecycleActor? LifecycleActor { get; set; }
    public string LifecycleActorName { get; set; } = string.Empty;
}
