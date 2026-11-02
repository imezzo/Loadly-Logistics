namespace LoadlyAggregator.Shared.Enums;

/// <summary>
/// Notification event types exposed on the API preference surface.
/// The enum name (e.g. TRIP_STARTED) is the canonical identifier used for
/// template keys, push payloads, and deduplication.
/// </summary>
public enum NotificationEventType
{
    TRIP_REQUESTED = 1,
    TRIP_SCHEDULED = 2,
    TRIP_STARTED = 3,
    TRIP_WAYPOINT_ARRIVED = 4,
    TRIP_WAYPOINT_LOADING_STARTED = 5,
    TRIP_WAYPOINT_LOADING_COMPLETED = 6,
    TRIP_WAYPOINT_UNLOADING_STARTED = 7,
    TRIP_WAYPOINT_UNLOADING_COMPLETED = 8,
    TRIP_WAYPOINT_DEPARTED = 9,
    TRIP_COMPLETED = 10,
    TRIP_CANCELLED = 11,
    TRIP_PUSHED_TO_CONTRACTORS = 12,
    TRIP_BOOKING_REQUEST_SUBMITTED = 13,
    TRIP_BOOKING_REQUEST_APPROVED = 14,
    TRIP_CONTRACTOR_UNASSIGNED = 15
}
