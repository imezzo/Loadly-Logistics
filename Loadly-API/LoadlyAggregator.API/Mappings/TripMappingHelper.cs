using System.Text.Json;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Entities.EventPayloads;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Mappings;

internal static class TripMappingHelper
{
    public static TripLocationDto? ExtractStartLocation(Trip trip)
    {
        if (trip.Recordings == null)
            return null;

        var tripStartedRecording = trip.Recordings
            .Where(r => r.RecordableType == RecordableType.TRIP && r.RecordableEventType == RecordableEventType.TRIP_STARTED)
            .OrderBy(r => r.CreatedAt)
            .FirstOrDefault();

        if (tripStartedRecording?.Data == null)
            return null;

        try
        {
            var eventData = JsonSerializer.Deserialize<DriverStartTripEventData>(tripStartedRecording.Data.RootElement);
            if (eventData?.Location == null)
                return null;

            return new TripLocationDto
            {
                Latitude = eventData.Location.Latitude,
                Longitude = eventData.Location.Longitude,
                RecordedAt = eventData.Location.RecordedAt
            };
        }
        catch
        {
            return null;
        }
    }

    public static string? ExtractCancellationReason(Trip trip)
    {
        if (trip.Recordings == null)
            return null;

        var tripCancelledRecording = trip.Recordings
            .Where(r => r.RecordableType == RecordableType.TRIP && r.RecordableEventType == RecordableEventType.TRIP_CANCELLED)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefault();

        if (tripCancelledRecording?.Data == null)
            return null;

        try
        {
            var eventData = JsonSerializer.Deserialize<DriverCancelTripEventData>(tripCancelledRecording.Data.RootElement);
            return eventData?.CancellationReason;
        }
        catch
        {
            return null;
        }
    }

    public static DateTime? ExtractCompletionTime(Trip trip)
    {
        if (trip.Recordings == null)
            return null;

        return trip.Recordings
            .Where(r => r.RecordableType == RecordableType.TRIP && r.RecordableEventType == RecordableEventType.TRIP_COMPLETED)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => (DateTime?)r.CreatedAt)
            .FirstOrDefault();
    }
}
