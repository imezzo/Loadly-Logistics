using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.DTOs.Relations;

/// <summary>Vehicle facility visit reference (e.g. on facility get/list).</summary>
public class VehicleFacilityVisitRelationDto
{
    public long Id { get; set; }
    public VehicleFacilityVisitEvent Event { get; set; }
    public string EventName { get; set; } = string.Empty;
    public VehicleFacilityVisitContext Context { get; set; }
    public string ContextName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
    public VehicleFacilityRelationDto? VehicleFacility { get; set; }
    public VehicleRelationDto? Vehicle { get; set; }
    public DriverRelationDto? Driver { get; set; }
    public AdminRelationDto? AddedBy { get; set; }
}

/// <summary>Vehicle facility visit on vehicle get/list — omits redundant vehicle self-relation.</summary>
public class VehicleFacilityVisitOnVehicleRelationDto
{
    public long Id { get; set; }
    public VehicleFacilityVisitEvent Event { get; set; }
    public string EventName { get; set; } = string.Empty;
    public VehicleFacilityVisitContext Context { get; set; }
    public string ContextName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
    public VehicleFacilityRelationDto? VehicleFacility { get; set; }
    public DriverRelationDto? Driver { get; set; }
    public AdminRelationDto? AddedBy { get; set; }
}
