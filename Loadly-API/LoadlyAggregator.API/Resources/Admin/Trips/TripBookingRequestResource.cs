using LoadlyAggregator.API.Resources.RelationResources;

namespace LoadlyAggregator.API.Resources.Admin.Trips;

public class TripBookingRequestResource
{
    public long Id { get; set; }
    public long TripId { get; set; }
    public long ContractorId { get; set; }
    public string? ContractorName { get; set; }
    public string? ContractorPhone { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public long? ReviewedByAdminId { get; set; }
}

public class ContractorTripBookingRequestResource : TripBookingRequestResource
{
    public TripRelationResource? Trip { get; set; }
    public CompanyRelationResource? Company { get; set; }
    public LeadRelationResource? Lead { get; set; }
    public TripContractDestinationResource? TripContractDestination { get; set; }
    public string VehicleTypeNameEn { get; set; } = string.Empty;
    public string VehicleTypeNameAr { get; set; } = string.Empty;
    public decimal? ContractorPrice { get; set; }
    public string? ContactPersonName { get; set; }
    public string? ContactPersonMobile { get; set; }
    public List<TripWaypointResource>? Waypoints { get; set; }
}
