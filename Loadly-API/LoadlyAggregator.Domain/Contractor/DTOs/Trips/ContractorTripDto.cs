using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.DTOs.Relations;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Contractor.DTOs.Trips;

public class ContractorTripDto
{
    public long Id { get; set; }
    public long? VehicleTypeId { get; set; }
    public long? VehicleId { get; set; }
    public long? DriverId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public TripStatus Status { get; set; }
    public string TripStatusName { get; set; } = string.Empty;
    public decimal? ContractDistanceKm { get; set; }
    public decimal? ActualDistanceKm { get; set; }
    public decimal? ActualCargoWeightTons { get; set; }
    public decimal? ContractorPrice { get; set; }
    public bool IsPaidToContractor { get; set; }
    public int? NumberOfDays { get; set; }
    public string? ContactPersonName { get; set; }
    public string? ContactPersonMobile { get; set; }
    public int? HelpersCount { get; set; }
    public int CutoffDuration { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DriverRelationDto? Driver { get; set; }
    public string VehicleTypeNameEn { get; set; } = string.Empty;
    public string VehicleTypeNameAr { get; set; } = string.Empty;
    public List<TripContractDestinationDto>? TripContractDestinations { get; set; }
    public VehicleRelationDto? Vehicle { get; set; }
    public CompanyRelationDto? Company { get; set; }
    public LeadRelationDto? Lead { get; set; }
    public ContractorRelationDto? Contractor { get; set; }
    public List<TripWaypointDto>? Waypoints { get; set; }
}
