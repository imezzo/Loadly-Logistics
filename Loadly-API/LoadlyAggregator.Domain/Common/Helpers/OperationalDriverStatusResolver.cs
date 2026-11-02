using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class OperationalDriverStatusResolver
{
    public static OperationalDriverStatus? Resolve(Trip trip) =>
        OperationalDriverStatusContext.TryBuild(trip)?.Resolve();

    internal static Recording? GetLatestOperationalRecording(Trip trip) =>
        (trip.Recordings ?? [])
            .Where(OperationalDriverStatusRules.IsOperationalRecording)
            .MaxBy(r => (r.CreatedAt, r.Id));

    public static bool IsOperationalRecording(Recording recording) =>
        OperationalDriverStatusRules.IsOperationalRecording(recording);

    internal static TripWaypoint? GetNextPendingWaypoint(Trip trip) =>
        trip.Waypoints?
            .Where(w => w.Status == TripWaypointStatus.Pending)
            .OrderBy(w => w.Sequence)
            .ThenBy(w => w.Id)
            .FirstOrDefault();

    internal static TripWaypoint? GetActiveWaypoint(Trip trip) =>
        trip.Waypoints?
            .Where(OperationalDriverStatusRules.IsActiveWaypoint)
            .OrderBy(w => w.Sequence)
            .ThenBy(w => w.Id)
            .FirstOrDefault();
}
