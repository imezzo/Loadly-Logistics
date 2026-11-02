using System.Text.Json;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Entities.EventPayloads;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.TripExtraFeePricing;

internal static class TripRecordingMetricsBuilder
{
    public static TripRecordingMetrics Build(IEnumerable<Recording>? recordings)
    {
        var items = recordings?.ToList() ?? [];
        var completePayload = ReadCompleteTripPayload(items);

        return new TripRecordingMetrics
        {
            WaitingMinutesAtStops = TotalDurationInMinutesByEventPair(
                items,
                RecordableEventType.WAYPOINT_ARRIVED,
                RecordableEventType.WAYPOINT_DEPARTED),
            LoadingDurationMinutes = TotalDurationInMinutesByEventPair(
                items,
                RecordableEventType.WAYPOINT_LOADING_STARTED,
                RecordableEventType.WAYPOINT_LOADING_COMPLETED),
            UnloadingDurationMinutes = TotalDurationInMinutesByEventPair(
                items,
                RecordableEventType.WAYPOINT_UNLOADING_STARTED,
                RecordableEventType.WAYPOINT_UNLOADING_COMPLETED),
            LoadingUnloadingCompletions = items.Count(r =>
                r.RecordableType == RecordableType.TRIP_WAYPOINT
                && (r.RecordableEventType == RecordableEventType.WAYPOINT_LOADING_COMPLETED
                    || r.RecordableEventType == RecordableEventType.WAYPOINT_UNLOADING_COMPLETED)),
            BillableTripHours = ActiveTripHoursFromRecordings(items),
            TollgatesTotalAmount = completePayload?.TollgatesTotalAmount,
            TollgatesWeightTotalAmount = completePayload?.TollgatesWeightTotalAmount
        };
    }

    private static DriverCompleteTripEventData? ReadCompleteTripPayload(IReadOnlyList<Recording> recordings)
    {
        var completed = recordings
            .Where(r =>
                r.RecordableType == RecordableType.TRIP
                && r.RecordableEventType == RecordableEventType.TRIP_COMPLETED
                && r.Data is not null)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefault();

        if (completed?.Data is null)
            return null;

        try
        {
            return JsonSerializer.Deserialize<DriverCompleteTripEventData>(completed.Data.RootElement);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static int TotalDurationInMinutesByEventPair(
        IReadOnlyList<Recording> recordings,
        RecordableEventType startedEventType,
        RecordableEventType completedEventType)
    {
        var events = recordings
            .Where(r =>
                r.RecordableType == RecordableType.TRIP_WAYPOINT
                && (r.RecordableEventType == startedEventType || r.RecordableEventType == completedEventType))
            .OrderBy(r => r.CreatedAt)
            .ToList();

        var startedAtStack = new Stack<DateTime>();
        var totalMinutes = 0;

        foreach (var ev in events)
        {
            if (ev.RecordableEventType == startedEventType)
            {
                startedAtStack.Push(ev.CreatedAt);
                continue;
            }

            if (startedAtStack.Count == 0)
            {
                continue;
            }

            var startedAt = startedAtStack.Pop();
            if (ev.CreatedAt <= startedAt)
            {
                continue;
            }

            totalMinutes += (int)Math.Ceiling((ev.CreatedAt - startedAt).TotalMinutes);
        }

        return totalMinutes;
    }

    private static decimal ActiveTripHoursFromRecordings(IReadOnlyList<Recording> recordings)
    {
        var startedAt = recordings
            .Where(r =>
                r.RecordableType == RecordableType.TRIP
                && r.RecordableEventType == RecordableEventType.TRIP_STARTED)
            .OrderBy(r => r.CreatedAt)
            .Select(r => (DateTime?)r.CreatedAt)
            .FirstOrDefault();

        var completedAt = recordings
            .Where(r =>
                r.RecordableType == RecordableType.TRIP
                && r.RecordableEventType == RecordableEventType.TRIP_COMPLETED)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => (DateTime?)r.CreatedAt)
            .FirstOrDefault();

        if (startedAt.HasValue && completedAt.HasValue && completedAt.Value > startedAt.Value)
        {
            return (decimal)(completedAt.Value - startedAt.Value).TotalHours;
        }

        return 0m;
    }
}
