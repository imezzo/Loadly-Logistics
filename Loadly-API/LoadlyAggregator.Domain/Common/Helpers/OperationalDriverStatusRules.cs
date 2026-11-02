using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class OperationalDriverStatusRules
{
    public static readonly TripWaypointStatus[] AtPickupStopStatuses =
    [
        TripWaypointStatus.Arrived,
        TripWaypointStatus.LoadingStarted,
        TripWaypointStatus.LoadingCompleted
    ];

    public static readonly TripWaypointStatus[] AtDropoffStopStatuses =
    [
        TripWaypointStatus.Arrived,
        TripWaypointStatus.UnloadingStarted,
        TripWaypointStatus.UnloadingCompleted
    ];

    public static readonly TripWaypointStatus[] AtDepotStopStatuses =
    [
        TripWaypointStatus.Arrived
    ];

    public static readonly RecordableEventType[] ExplicitOverrideEvents =
    [
        RecordableEventType.WAYPOINT_LOADING_STARTED,
        RecordableEventType.WAYPOINT_UNLOADING_STARTED,
        RecordableEventType.TRIP_CUSTOMS_ENTERED,
        RecordableEventType.TRIP_CUSTOMS_EXITED
    ];

    public static readonly RecordableType[] OperationalRecordableTypes =
    [
        RecordableType.TRIP,
        RecordableType.TRIP_WAYPOINT
    ];

    public static bool IsOperationalRecording(Recording recording) =>
        OperationalRecordableTypes.Contains(recording.RecordableType)
        && recording.RecordableEventType != RecordableEventType.TRIP_CANCELLED
        && recording.RecordableEventType != RecordableEventType.TRIP_COMPLETED;

    public static bool IsAtPickupStop(TripWaypoint waypoint) =>
        waypoint.StopType == TripWaypointStopType.Pickup && AtPickupStopStatuses.Contains(waypoint.Status);

    public static bool IsAtDropoffStop(TripWaypoint waypoint) =>
        waypoint.StopType == TripWaypointStopType.Dropoff && AtDropoffStopStatuses.Contains(waypoint.Status);

    public static bool IsAtDepotStop(TripWaypoint waypoint) =>
        waypoint.StopType == TripWaypointStopType.Depot && AtDepotStopStatuses.Contains(waypoint.Status);

    public static bool IsActiveWaypoint(TripWaypoint waypoint) =>
        waypoint.Status != TripWaypointStatus.Completed;

    public static OperationalDriverStatus? MapExplicitEventStatus(RecordableEventType eventType) =>
        eventType switch
        {
            RecordableEventType.WAYPOINT_LOADING_STARTED => OperationalDriverStatus.LoadingInProgress,
            RecordableEventType.WAYPOINT_UNLOADING_STARTED => OperationalDriverStatus.UnloadingInProgress,
            RecordableEventType.TRIP_CUSTOMS_ENTERED => OperationalDriverStatus.EnterCustoms,
            RecordableEventType.TRIP_CUSTOMS_EXITED => OperationalDriverStatus.ExitCustoms,
            _ => null
        };
}
