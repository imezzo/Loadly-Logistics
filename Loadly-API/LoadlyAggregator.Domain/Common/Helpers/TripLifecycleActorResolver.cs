using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class TripLifecycleActorResolver
{
    public static TripLifecycleActor? Resolve(Trip trip)
    {
        if (trip.Status is not TripStatus.InProgress and not TripStatus.Completed)
            return null;

        var lifecycle = (trip.Recordings ?? [])
            .Where(IsLifecycleRecording)
            .ToList();

        int total = lifecycle.Count;
        if (total == 0)
            return TripLifecycleActor.Dispatcher;

        int driverEvents = lifecycle.Count(r => r.AddedByModelType == "Driver");
        int adminEvents = lifecycle.Count(r => r.AddedByModelType == "Admin");

        if (driverEvents == total)
            return TripLifecycleActor.Driver;

        if (adminEvents == total)
            return TripLifecycleActor.Dispatcher;

        if (driverEvents > 0 && adminEvents > 0)
            return TripLifecycleActor.Assisted;

        return TripLifecycleActor.Dispatcher;
    }

    internal static bool IsLifecycleRecording(Recording recording) =>
        (recording.RecordableType == RecordableType.TRIP || recording.RecordableType == RecordableType.TRIP_WAYPOINT)
        && recording.RecordableEventType <= RecordableEventType.WAYPOINT_UNLOADING_COMPLETED;
}
