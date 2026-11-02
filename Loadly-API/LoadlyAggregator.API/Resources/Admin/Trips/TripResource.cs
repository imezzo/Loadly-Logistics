using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Resources.Admin.Trips;


public class TripResource
{
    public long Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
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
    public DateOnly? FinancialDate { get; set; }
    public int Status { get; set; }
    public string TripStatusName { get; set; } = string.Empty;
    public int? CurrentDriverStatus { get; set; }
    public string CurrentDriverStatusName { get; set; } = string.Empty;
    public int TripDocumentStatus { get; set; }
    public string TripDocumentStatusName { get; set; } = string.Empty;
    public int CustomsStatus { get; set; }
    public string CustomsStatusName { get; set; } = string.Empty;
    public decimal? CustomsTotalAmount { get; set; }
    public bool HasCustoms => CustomsStatus != (int)TripCustomsStatus.None;
    public decimal? ContractDistanceKm { get; set; }
    public decimal? ActualDistanceKm { get; set; }
    public decimal? ActualCargoWeightTons { get; set; }
    public decimal? CargoPrice { get; set; }
    public decimal? SpotPrice { get; set; }
    public int? NumberOfDays { get; set; }
    public int? OverNightCostBearer { get; set; }
    public string OverNightCostBearerName { get; set; } = string.Empty;
    public int? DispatchingMode { get; set; }
    public string DispatchingModeName { get; set; } = string.Empty;
    public int? Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public decimal? TotalAddonsCost { get; set; }
    public decimal? TotalTripCost { get; set; }
    public decimal? ContractorPrice { get; set; }
    public decimal? LoadlyCommission =>
        TotalTripCost.HasValue && ContractorPrice.HasValue
            ? TotalTripCost.Value - ContractorPrice.Value
            : null;
    public decimal? TripCostWithoutAddon { get; set; }
    public string? CancellationReason { get; set; }
    public bool IsExternalTrip { get; set; } = false;
    public bool? IsBackfill { get; set; }
    public string? ContactPersonName { get; set; }
    public string? ContactPersonMobile { get; set; }
    public string? DispatcherNotes { get; set; }
    public long? DispatcherId { get; set; }
    public AdminRelationResource? Dispatcher { get; set; }
    public string? ProofOfDelivery { get; set; }
    public long? AuditedById { get; set; }
    public DateTime? AuditedAt { get; set; }
    public AdminRelationResource? AuditedBy { get; set; }
    public bool IsEditable { get; set; } = true;
    public ContractConditionsDocumentDto? ContractConditions { get; set; }
    public int? HelpersCount { get; set; }
    public int CutoffDuration { get; set; } = TripConstants.DefaultCutoffDuration;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DriverRelationResource? Driver { get; set; }
    public ContractorRelationResource? Contractor { get; set; }
    public ContractRelationResource? Contract { get; set; }
    public ContractVehicleTypeRelationResource? ContractVehicleType { get; set; }
    public List<TripContractDestinationResource>? TripContractDestinations { get; set; }
    public VehicleRelationResource? Vehicle { get; set; }
    public TripVehicleAssignmentSnapshotResource? VehicleSnapshot { get; set; }
    public bool IsHeadVehiclePlateChanged { get; set; }
    public bool IsTailVehiclePlateChanged { get; set; }
    public DateTime? HeadPlateChangedAt { get; set; }
    public DateTime? TailPlateChangedAt { get; set; }
    public CoupledVehicleRelationResource? TailVehicle { get; set; }
    public CompanyRelationResource? Company { get; set; }
    public LeadRelationResource? Lead { get; set; }
    public List<TripWaypointResource>? Waypoints { get; set; }
    public List<TripRecordingResource>? Recordings { get; set; }
    public List<TripDriverDocumentResource>? Documents { get; set; }
    public List<TripDocumentResource>? CollectorDocuments { get; set; }
    public List<TripChargeResource>? TripCharges { get; set; }
    public List<TripBookingRequestResource>? BookingRequests { get; set; }
    public TripLocationResource? CurrentLocation { get; set; }
    public TripLocationResource? StartLocation { get; set; }
    public InvoiceRelationResource? Invoice { get; set; }
    public InvoiceRelationResource? ClaimInvoice { get; set; }
    public int InvoicingStatus { get; set; }
    public string InvoicingStatusName { get; set; } = string.Empty;
    public bool RequiresMainInvoice { get; set; }
    public bool RequiresClaimInvoice { get; set; }
    public bool FullyInvoiced { get; set; }
    public decimal TotalBillableAmount { get; set; }
    public decimal TotalBillableTripItemsAmount { get; set; }
    public decimal TotalBillableClaimItemsAmount { get; set; }
    public decimal TotalFuelAmount { get; set; }
    public decimal TotalBillableFuelAmount { get; set; }
    public int? FuelPricingCalculationOption { get; set; }
    public string FuelPricingCalculationOptionName { get; set; } = string.Empty;
    public decimal TotalBillableTollGatesAmount { get; set; }
    public int? LifecycleActor { get; set; }
    public string LifecycleActorName { get; set; } = string.Empty;
}

public class TripWaypointResource
{
    public long Id { get; set; }
    public int Sequence { get; set; }
    public int StopType { get; set; }  // 0 = Pickup, 1 = Dropoff, 2 = Depot
    public int Status { get; set; }  // 0 = Pending, 1 = Arrived, 2 = Completed, 3 = Failed
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? AddressName { get; set; }
    public string? GooglePlaceId { get; set; }
    public string? TaskDetails { get; set; }

    public static TripWaypointResource FromDto(TripWaypointDto dto) => new()
    {
        Id = dto.Id,
        Sequence = dto.Sequence,
        StopType = (int)dto.StopType,
        Status = (int)dto.Status,
        Latitude = dto.Latitude,
        Longitude = dto.Longitude,
        AddressName = dto.AddressName,
        GooglePlaceId = dto.GooglePlaceId,
        TaskDetails = dto.TaskDetails
    };
}

public class TripRecordingResource
{
    public long Id { get; set; }
    public int EventType { get; set; }  // 1=TRIP_STARTED, 2=WAYPOINT_ARRIVED, 3=WAYPOINT_DEPARTED, 4=TRIP_COMPLETED
    public string EventName { get; set; } = string.Empty;
    public long? AddedBy { get; set; }
    public string? AddedByModelType { get; set; }
    public System.Text.Json.JsonElement? Data { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TripChargeExtraFeeTypeResource
{
    public int Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
}

public class TripChargeResource
{
    public long Id { get; set; }
    public int ChargeType { get; set; }
    public string ChargeTypeName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal? CompanyShare { get; set; }
    public int? ExtraFeeTypeId { get; set; }
    public string? Notes { get; set; }
    public TripChargeExtraFeeTypeResource? ExtraFeeType { get; set; }
}

public class TripDriverDocumentResource
{
    public string Type { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public List<string> Paths { get; set; } = [];
    public List<string> FileUrls { get; set; } = [];
    public long RecordingId { get; set; }
    public long? WaypointId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TripDocumentResource
{
    public long DocumentId { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public bool Required { get; set; }
    public List<string> Paths { get; set; } = [];
    public List<string> FileUrls { get; set; } = [];
    public int? Status { get; set; }
    public string? StatusName { get; set; }
    public string? Note { get; set; }
    public long? AddedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class TripLocationResource
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public DateTime RecordedAt { get; set; }
}
