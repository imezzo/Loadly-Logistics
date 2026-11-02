using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

internal sealed class OperationalDriverStatusContext
{
    private OperationalDriverStatusContext(
        Recording latest,
        TripWaypoint? nextPending,
        TripWaypoint? active,
        bool isTripJustStarted)
    {
        Latest = latest;
        NextPending = nextPending;
        Active = active;
        IsTripJustStarted = isTripJustStarted;
    }

    public Recording Latest { get; }

    public TripWaypoint? NextPending { get; }

    public TripWaypoint? Active { get; }

    public bool IsTripJustStarted { get; }

    public static OperationalDriverStatusContext? TryBuild(Trip trip)
    {
        if (trip.Status != TripStatus.InProgress)
            return null;

        var operationalRecordings = (trip.Recordings ?? [])
            .Where(OperationalDriverStatusRules.IsOperationalRecording)
            .ToList();

        var latest = operationalRecordings.MaxBy(r => (r.CreatedAt, r.Id));
        if (latest == null)
            return null;

        var nextPending = trip.Waypoints?
            .Where(w => w.Status == TripWaypointStatus.Pending)
            .OrderBy(w => w.Sequence)
            .ThenBy(w => w.Id)
            .FirstOrDefault();

        var active = trip.Waypoints?
            .Where(OperationalDriverStatusRules.IsActiveWaypoint)
            .OrderBy(w => w.Sequence)
            .ThenBy(w => w.Id)
            .FirstOrDefault();

        var isTripJustStarted = operationalRecordings.Count == 1
            && latest.RecordableEventType == RecordableEventType.TRIP_STARTED;

        return new OperationalDriverStatusContext(latest, nextPending, active, isTripJustStarted);
    }

    public OperationalDriverStatus? Resolve()
    {
        var explicitStatus = OperationalDriverStatusRules.MapExplicitEventStatus(Latest.RecordableEventType);
        if (explicitStatus.HasValue)
            return explicitStatus;

        if (IsHeadingTo(TripWaypointStopType.Pickup))
            return OperationalDriverStatus.HeadingToPickup;

        if (IsHeadingTo(TripWaypointStopType.Dropoff))
            return OperationalDriverStatus.HeadingToDropoff;

        if (IsHeadingTo(TripWaypointStopType.Depot))
            return OperationalDriverStatus.HeadingToDepot;

        return ResolveFromActiveWaypoint();
    }

    private bool IsHeadingTo(TripWaypointStopType stopType)
    {
        if (NextPending?.StopType != stopType)
            return false;

        if (IsTripJustStarted && Latest.RecordableEventType == RecordableEventType.TRIP_STARTED)
            return true;

        return Latest.RecordableEventType == RecordableEventType.WAYPOINT_DEPARTED;
    }

    private OperationalDriverStatus? ResolveFromActiveWaypoint()
    {
        if (Active == null)
            return null;

        if (OperationalDriverStatusRules.IsAtPickupStop(Active))
            return OperationalDriverStatus.LoadingInProgress;

        if (OperationalDriverStatusRules.IsAtDropoffStop(Active))
            return OperationalDriverStatus.UnloadingInProgress;

        if (OperationalDriverStatusRules.IsAtDepotStop(Active))
            return OperationalDriverStatus.AtDepot;

        return Active.StopType switch
        {
            TripWaypointStopType.Pickup => OperationalDriverStatus.HeadingToPickup,
            TripWaypointStopType.Dropoff => OperationalDriverStatus.HeadingToDropoff,
            TripWaypointStopType.Depot => OperationalDriverStatus.HeadingToDepot,
            _ => null
        };
    }
}
