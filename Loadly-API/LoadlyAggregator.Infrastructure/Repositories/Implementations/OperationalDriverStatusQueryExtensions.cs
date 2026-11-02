using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

internal static class OperationalDriverStatusQueryExtensions
{
    internal static IQueryable<Trip> WhereLatestOperationalEventIs(
        this IQueryable<Trip> query,
        RecordableEventType eventType) =>
        query.Where(t =>
            t.Recordings!
                .Where(r =>
                    OperationalDriverStatusRules.OperationalRecordableTypes.Contains(r.RecordableType)
                    && r.RecordableEventType != RecordableEventType.TRIP_CANCELLED
                    && r.RecordableEventType != RecordableEventType.TRIP_COMPLETED)
                .OrderByDescending(r => r.CreatedAt)
                .ThenByDescending(r => r.Id)
                .Select(r => r.RecordableEventType)
                .FirstOrDefault() == eventType);

    internal static IQueryable<Trip> WhereLatestOperationalEventNotIn(
        this IQueryable<Trip> query,
        RecordableEventType[] excludedEvents) =>
        query.Where(t => !excludedEvents.Contains(
            t.Recordings!
                .Where(r =>
                    OperationalDriverStatusRules.OperationalRecordableTypes.Contains(r.RecordableType)
                    && r.RecordableEventType != RecordableEventType.TRIP_CANCELLED
                    && r.RecordableEventType != RecordableEventType.TRIP_COMPLETED)
                .OrderByDescending(r => r.CreatedAt)
                .ThenByDescending(r => r.Id)
                .Select(r => r.RecordableEventType)
                .FirstOrDefault()));

    internal static IQueryable<Trip> WhereTripJustStartedOrLatestDeparted(this IQueryable<Trip> query) =>
        query.Where(t =>
            (!t.Recordings!.Any(r =>
                OperationalDriverStatusRules.OperationalRecordableTypes.Contains(r.RecordableType)
                && r.RecordableEventType != RecordableEventType.TRIP_CANCELLED
                && r.RecordableEventType != RecordableEventType.TRIP_COMPLETED
                && r.RecordableEventType != RecordableEventType.TRIP_STARTED)
                && t.Recordings!.Any(r =>
                    OperationalDriverStatusRules.OperationalRecordableTypes.Contains(r.RecordableType)
                    && r.RecordableEventType == RecordableEventType.TRIP_STARTED))
            || t.Recordings!
                .Where(r =>
                    OperationalDriverStatusRules.OperationalRecordableTypes.Contains(r.RecordableType)
                    && r.RecordableEventType != RecordableEventType.TRIP_CANCELLED
                    && r.RecordableEventType != RecordableEventType.TRIP_COMPLETED)
                .OrderByDescending(r => r.CreatedAt)
                .ThenByDescending(r => r.Id)
                .Select(r => r.RecordableEventType)
                .FirstOrDefault() == RecordableEventType.WAYPOINT_DEPARTED);

    internal static IQueryable<Trip> WhereHasPendingStopType(
        this IQueryable<Trip> query,
        TripWaypointStopType stopType) =>
        query.Where(t => t.Waypoints!.Any(w =>
            w.Status == TripWaypointStatus.Pending && w.StopType == stopType));

    internal static IQueryable<Trip> WhereAtStopPhaseOrLatestStartedEvent(
        this IQueryable<Trip> query,
        TripWaypointStopType stopType,
        TripWaypointStatus[] atStopStatuses,
        RecordableEventType startedEvent) =>
        query.Where(t =>
            t.Recordings!
                .Where(r =>
                    OperationalDriverStatusRules.OperationalRecordableTypes.Contains(r.RecordableType)
                    && r.RecordableEventType != RecordableEventType.TRIP_CANCELLED
                    && r.RecordableEventType != RecordableEventType.TRIP_COMPLETED)
                .OrderByDescending(r => r.CreatedAt)
                .ThenByDescending(r => r.Id)
                .Select(r => r.RecordableEventType)
                .FirstOrDefault() == startedEvent
            || (
                !OperationalDriverStatusRules.ExplicitOverrideEvents.Contains(
                    t.Recordings!
                        .Where(r =>
                            OperationalDriverStatusRules.OperationalRecordableTypes.Contains(r.RecordableType)
                            && r.RecordableEventType != RecordableEventType.TRIP_CANCELLED
                            && r.RecordableEventType != RecordableEventType.TRIP_COMPLETED)
                        .OrderByDescending(r => r.CreatedAt)
                        .ThenByDescending(r => r.Id)
                        .Select(r => r.RecordableEventType)
                        .FirstOrDefault())
                && t.Waypoints!.Any(active =>
                    active.StopType == stopType
                    && atStopStatuses.Contains(active.Status)
                    && !t.Waypoints!.Any(earlier =>
                        earlier.Status != TripWaypointStatus.Completed
                        && (earlier.Sequence < active.Sequence
                            || (earlier.Sequence == active.Sequence && earlier.Id < active.Id))))));
}
