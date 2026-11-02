using LoadlyAggregator.API.Resources.Admin.Trips;
using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Resources.Contractor.Trips;

public class ContractorTripResource
{
    public long Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
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
    public decimal? ContractorPrice { get; set; }
    public bool IsPaidToContractor { get; set; }
    public int? NumberOfDays { get; set; }
    public string? ContactPersonName { get; set; }
    public string? ContactPersonMobile { get; set; }
    public int? HelpersCount { get; set; }
    public int CutoffDuration { get; set; } = TripConstants.DefaultCutoffDuration;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DriverRelationResource? Driver { get; set; }
    public string VehicleTypeNameEn { get; set; } = string.Empty;
    public string VehicleTypeNameAr { get; set; } = string.Empty;
    public List<TripContractDestinationResource>? TripContractDestinations { get; set; }
    public VehicleRelationResource? Vehicle { get; set; }
    public CompanyRelationResource? Company { get; set; }
    public LeadRelationResource? Lead { get; set; }
    public ContractorRelationResource? Contractor { get; set; }
    public List<TripWaypointResource>? Waypoints { get; set; }
}
