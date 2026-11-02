using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

internal static class OperationalDriverStatusQueryFilter
{
    internal static IQueryable<Trip> Apply(
        IQueryable<Trip> query,
        OperationalDriverStatus status) =>
        status switch
        {
            OperationalDriverStatus.HeadingToPickup => ApplyHeadingToStopTypeFilter(query, TripWaypointStopType.Pickup),
            OperationalDriverStatus.LoadingInProgress => query.WhereAtStopPhaseOrLatestStartedEvent(
                TripWaypointStopType.Pickup,
                OperationalDriverStatusRules.AtPickupStopStatuses,
                RecordableEventType.WAYPOINT_LOADING_STARTED),
            OperationalDriverStatus.HeadingToDropoff => ApplyHeadingToStopTypeFilter(query, TripWaypointStopType.Dropoff),
            OperationalDriverStatus.UnloadingInProgress => query.WhereAtStopPhaseOrLatestStartedEvent(
                TripWaypointStopType.Dropoff,
                OperationalDriverStatusRules.AtDropoffStopStatuses,
                RecordableEventType.WAYPOINT_UNLOADING_STARTED),
            OperationalDriverStatus.EnterCustoms => query.WhereLatestOperationalEventIs(RecordableEventType.TRIP_CUSTOMS_ENTERED),
            OperationalDriverStatus.ExitCustoms => query.WhereLatestOperationalEventIs(RecordableEventType.TRIP_CUSTOMS_EXITED),
            OperationalDriverStatus.HeadingToDepot => ApplyHeadingToStopTypeFilter(query, TripWaypointStopType.Depot),
            OperationalDriverStatus.AtDepot => query.WhereAtStopPhaseOrLatestStartedEvent(
                TripWaypointStopType.Depot,
                OperationalDriverStatusRules.AtDepotStopStatuses,
                RecordableEventType.WAYPOINT_ARRIVED),
            _ => query.Where(_ => false)
        };

    private static IQueryable<Trip> ApplyHeadingToStopTypeFilter(IQueryable<Trip> query, TripWaypointStopType stopType) =>
        query
            .WhereHasPendingStopType(stopType)
            .WhereLatestOperationalEventNotIn(OperationalDriverStatusRules.ExplicitOverrideEvents)
            .WhereTripJustStartedOrLatestDeparted();
}
