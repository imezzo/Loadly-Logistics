using LoadlyAggregator.API.Resources.Admin.Trips;
using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Resources.Company.Trips;

public class CompanyTripResource
{
    public long Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public long ContractId { get; set; }
    public long? ContractVehicleTypeId { get; set; }
    public long? VehicleTypeId { get; set; }
    public long? VehicleId { get; set; }
    public long? DriverId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Status { get; set; }
    public string TripStatusName { get; set; } = string.Empty;
    public int? CurrentDriverStatus { get; set; }
    public string CurrentDriverStatusName { get; set; } = string.Empty;
    public int TripDocumentStatus { get; set; }
    public string TripDocumentStatusName { get; set; } = string.Empty;
    public decimal? ContractDistanceKm { get; set; }
    public decimal? ActualDistanceKm { get; set; }
    public decimal? ActualCargoWeightTons { get; set; }
    public decimal? CargoPrice { get; set; }
    public decimal? SpotPrice { get; set; }
    public int? NumberOfDays { get; set; }
    public decimal? TotalAddonsCost { get; set; }
    public decimal? TotalTripCost { get; set; }
    public decimal? ContractorPrice { get; set; }
    public decimal? TripCostWithoutAddon { get; set; }
    public string? ContactPersonName { get; set; }
    public string? ContactPersonMobile { get; set; }
    public int? HelpersCount { get; set; }
    public int CutoffDuration { get; set; } = TripConstants.DefaultCutoffDuration;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DriverRelationResource? Driver { get; set; }
    public ContractRelationResource? Contract { get; set; }
    public ContractVehicleTypeRelationResource? ContractVehicleType { get; set; }
    public List<TripContractDestinationResource>? TripContractDestinations { get; set; }
    public VehicleRelationResource? Vehicle { get; set; }
    public CompanyRelationResource? Company { get; set; }
    public List<TripWaypointResource>? Waypoints { get; set; }
    public List<TripRecordingResource>? Recordings { get; set; }
    public List<TripDriverDocumentResource>? Documents { get; set; }
    public List<TripDocumentResource>? CollectorDocuments { get; set; }
    public List<TripChargeResource>? TripCharges { get; set; }
    public TripLocationResource? CurrentLocation { get; set; }
    public TripLocationResource? StartLocation { get; set; }
    public InvoiceRelationResource? Invoice { get; set; }
    public InvoiceRelationResource? ClaimInvoice { get; set; }
    public int? LifecycleActor { get; set; }
    public string LifecycleActorName { get; set; } = string.Empty;
}
