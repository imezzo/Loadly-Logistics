using LoadlyAggregator.API.Resources.Admin.Trips;
using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Resources.Driver.Trips;

public class DriverTripResource
{
    public long Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public CompanyRelationResource? Company { get; set; }
    public VehicleRelationResource? Vehicle { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? CompletionTime { get; set; }
    public int Status { get; set; }
    public int? CurrentDriverStatus { get; set; }
    public string CurrentDriverStatusName { get; set; } = string.Empty;
    public int CustomsStatus { get; set; }
    public string CustomsStatusName { get; set; } = string.Empty;
    public string? ContactPersonName { get; set; }
    public string? ContactPersonMobile { get; set; }
    public int? HelpersCount { get; set; }
    public int CutoffDuration { get; set; } = TripConstants.DefaultCutoffDuration;
    public DateTime CreatedAt { get; set; }
    public List<TripWaypointResource>? Waypoints { get; set; }
    public List<TripDriverDocumentResource>? Documents { get; set; }
    public List<TripDocumentResource>? CollectorDocuments { get; set; }
    public TripLocationResource? CurrentLocation { get; set; }
    public TripLocationResource? StartLocation { get; set; }
    public int? LifecycleActor { get; set; }
    public string LifecycleActorName { get; set; } = string.Empty;
}
