using LoadlyAggregator.API.Resources.Admin.Trips;
using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Resources.Collector.Trips;

/// <summary>
/// Minimal projection of a trip for the collector workflow. Exposes only the fields a collector
/// needs to identify the trip and perform document collection (core info, waypoints, and
/// collector documents).
/// </summary>
public class CollectorTripResource
{
    public long Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public long? ContractId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Status { get; set; }
    public int TripDocumentStatus { get; set; }
    public string TripDocumentStatusName { get; set; } = string.Empty;
    public int CustomsStatus { get; set; }
    public bool HasCustoms => CustomsStatus != (int)TripCustomsStatus.None;
    public string? ContactPersonName { get; set; }
    public string? ContactPersonMobile { get; set; }
    public int CutoffDuration { get; set; } = TripConstants.DefaultCutoffDuration;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public CompanyRelationResource? Company { get; set; }
    public VehicleRelationResource? Vehicle { get; set; }
    public DriverRelationResource? Driver { get; set; }
    public List<TripWaypointResource>? Waypoints { get; set; }
    public List<TripDocumentResource>? CollectorDocuments { get; set; }
    public int? LifecycleActor { get; set; }
    public string LifecycleActorName { get; set; } = string.Empty;
}
