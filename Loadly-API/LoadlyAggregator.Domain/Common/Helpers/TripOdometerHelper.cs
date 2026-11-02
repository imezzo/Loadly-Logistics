using System.Text.Json;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Entities.EventPayloads;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class TripOdometerHelper
{
    public static int? GetTripStartOdometerReading(IEnumerable<Recording> tripRecordings)
    {
        List<(DateTime At, int Odometer, int Sequence)> rows = CollectDepartOdometerRows(tripRecordings);
        if (rows.Count == 0)
            return null;

        return ResolveStartOdometer(rows);
    }

    public static decimal? CalculateActualDistanceKm(
        IEnumerable<Recording> tripRecordings,
        int tripCompleteOdometerReading)
    {
        int? start = GetTripStartOdometerReading(tripRecordings);
        if (!start.HasValue)
            return null;

        int distance = tripCompleteOdometerReading - start.Value;
        return distance < 0 ? 0 : distance;
    }

    private static List<(DateTime At, int Odometer, int Sequence)> CollectDepartOdometerRows(
        IEnumerable<Recording> tripRecordings)
    {
        var list = new List<(DateTime At, int Odometer, int Sequence)>();
        foreach (Recording r in tripRecordings
                     .Where(x => x.RecordableType == RecordableType.TRIP_WAYPOINT
                         && x.RecordableEventType == RecordableEventType.WAYPOINT_DEPARTED
                         && x.Data != null)
                     .OrderBy(x => x.CreatedAt))
        {
            if (!TryDeserializeDepart(r, out DriverDepartFromWaypointEventData? data) || data is null)
                continue;
            if (data.Odometer is not int o || o < 0)
                continue;
            int sequence = data.Waypoint?.Sequence ?? 0;
            list.Add((r.CreatedAt, o, sequence));
        }

        return list;
    }

    private static int ResolveStartOdometer(List<(DateTime At, int Odometer, int Sequence)> rows)
    {
        int minS = rows.Min(x => x.Sequence);
        return rows.Where(x => x.Sequence == minS).OrderBy(x => x.At).First().Odometer;
    }

    private static bool TryDeserializeDepart(Recording r, out DriverDepartFromWaypointEventData? data)
    {
        data = null;
        if (r.Data == null)
            return false;
        try
        {
            data = JsonSerializer.Deserialize<DriverDepartFromWaypointEventData>(r.Data.RootElement);
            return data != null;
        }
        catch
        {
            return false;
        }
    }
}
