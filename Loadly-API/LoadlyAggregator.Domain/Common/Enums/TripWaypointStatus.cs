namespace LoadlyAggregator.Domain.Common.Enums;

public enum TripWaypointStatus
{
    Pending = 0,
    Arrived = 1,
    LoadingStarted = 2,
    LoadingCompleted = 3,
    Departed = 4,
    Completed = 5,
    Failed = 6,
    UnloadingStarted = 7,
    UnloadingCompleted = 8
}
