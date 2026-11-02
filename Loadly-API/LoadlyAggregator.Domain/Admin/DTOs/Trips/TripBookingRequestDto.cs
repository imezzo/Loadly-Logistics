using LoadlyAggregator.Domain.Common.DTOs.Relations;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class TripBookingRequestDto
{
    public long Id { get; set; }
    public long TripId { get; set; }
    public long ContractorId { get; set; }
    public string? ContractorName { get; set; }
    public string? ContractorPhone { get; set; }
    public TripBookingRequestStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public long? ReviewedByAdminId { get; set; }
}

public class ContractorTripBookingRequestDto : TripBookingRequestDto
{
    public TripRelationDto? Trip { get; set; }
    public CompanyRelationDto? Company { get; set; }
    public LeadRelationDto? Lead { get; set; }
    public TripContractDestinationDto? TripContractDestination { get; set; }
    public string VehicleTypeNameEn { get; set; } = string.Empty;
    public string VehicleTypeNameAr { get; set; } = string.Empty;
    public decimal? ContractorPrice { get; set; }
    public string? ContactPersonName { get; set; }
    public string? ContactPersonMobile { get; set; }
    public List<TripWaypointDto>? Waypoints { get; set; }
}

public class ApproveTripBookingRequestDto
{
    public long TripId { get; set; }
    public long RequestId { get; set; }
}
