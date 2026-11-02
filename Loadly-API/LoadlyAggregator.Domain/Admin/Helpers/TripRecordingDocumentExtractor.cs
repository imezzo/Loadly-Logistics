using System.Text.Json;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.Helpers;

public static class TripRecordingDocumentExtractor
{
    public static void PopulateDocumentsFromRecordings(TripDto tripDto)
    {
        ArgumentNullException.ThrowIfNull(tripDto);

        if (tripDto.Recordings is null)
            return;

        tripDto.Documents = tripDto.Recordings.Count == 0
            ? []
            : ExtractFromRecordings(tripDto.Recordings);
    }

    public static List<TripDriverDocumentDto> ExtractFromRecordings(IReadOnlyList<TripRecordingDto> recordings)
    {
        var documents = new List<TripDriverDocumentDto>();
        foreach (var r in recordings)
        {
            if (!r.Data.HasValue)
                continue;

            var data = r.Data.Value;
            var recordingId = r.Id;
            var createdAt = r.CreatedAt;
            long? waypointId = data.TryGetProperty("Waypoint", out var waypointEl) && waypointEl.TryGetProperty("Id", out var wpId)
                ? wpId.GetInt64()
                : null;

            switch ((RecordableEventType)r.EventType)
            {
                case RecordableEventType.WAYPOINT_UNLOADING_COMPLETED:
                    AddSinglePathFromString(documents, data, "DeliveryNoteImagePath", TripDriverDocumentType.DeliveryNote, recordingId, waypointId, createdAt);
                    AddPathArray(documents, data, "UnloadingImagePaths", TripDriverDocumentType.UnloadingImages, recordingId, waypointId, createdAt);
                    AddSinglePathFromString(documents, data, "OdometerImagePath", TripDriverDocumentType.UnloadingOdometer, recordingId, waypointId, createdAt);
                    break;

                case RecordableEventType.WAYPOINT_LOADING_COMPLETED:
                    AddSinglePathFromString(documents, data, "ReceiptNoteImagePath", TripDriverDocumentType.ReceiptNote, recordingId, waypointId, createdAt);
                    AddPathArray(documents, data, "ShipmentImagePaths", TripDriverDocumentType.ShipmentImages, recordingId, waypointId, createdAt);
                    break;

                case RecordableEventType.TRIP_STARTED:
                    AddSinglePathFromString(documents, data, "OdometerImagePath", TripDriverDocumentType.OdometerStart, recordingId, waypointId, createdAt);
                    break;

                case RecordableEventType.WAYPOINT_DEPARTED:
                    AddSinglePathFromString(documents, data, "DeliveryNoteImagePath", TripDriverDocumentType.DeliveryNote, recordingId, waypointId, createdAt);
                    AddSinglePathFromString(documents, data, "OdometerImagePath", TripDriverDocumentType.OdometerStart, recordingId, waypointId, createdAt);
                    break;

                case RecordableEventType.TRIP_COMPLETED:
                    AddSinglePathFromString(documents, data, "OdometerImagePath", TripDriverDocumentType.OdometerEnd, recordingId, waypointId, createdAt);
                    AddPathArray(documents, data, "TollgateImagePaths", TripDriverDocumentType.TollgateImages, recordingId, waypointId, createdAt);
                    AddPathArray(documents, data, "TollgateWeightImagePaths", TripDriverDocumentType.TollgateWeightImages, recordingId, waypointId, createdAt);
                    break;

                case RecordableEventType.TRIP_CUSTOMS_ENTERED:
                    AddPathArray(documents, data, "CustomsDocumentsPaths", TripDriverDocumentType.CustomsDocuments, recordingId, waypointId, createdAt);
                    break;

                case RecordableEventType.TRIP_CUSTOMS_EXITED:
                    AddPathArray(documents, data, "CustomsReceiptPaths", TripDriverDocumentType.CustomsReceipt, recordingId, waypointId, createdAt);
                    break;

            }
        }

        return documents;
    }

    private static void AddSinglePathFromString(
        List<TripDriverDocumentDto> documents,
        JsonElement data,
        string property,
        TripDriverDocumentType type,
        long recordingId,
        long? waypointId,
        DateTime createdAt)
    {
        if (!data.TryGetProperty(property, out var el) || el.ValueKind != JsonValueKind.String)
            return;

        var path = el.GetString();
        if (string.IsNullOrEmpty(path))
            return;

        documents.Add(new TripDriverDocumentDto
        {
            Type = type,
            Paths = [path],
            RecordingId = recordingId,
            WaypointId = waypointId,
            CreatedAt = createdAt
        });
    }

    private static void AddPathArray(
        List<TripDriverDocumentDto> documents,
        JsonElement data,
        string property,
        TripDriverDocumentType type,
        long recordingId,
        long? waypointId,
        DateTime createdAt)
    {
        if (!data.TryGetProperty(property, out var arr) || arr.ValueKind != JsonValueKind.Array)
            return;

        var paths = new List<string>();
        foreach (var item in arr.EnumerateArray())
        {
            var p = item.GetString();
            if (!string.IsNullOrEmpty(p))
                paths.Add(p);
        }
        if (paths.Count == 0)
            return;

        documents.Add(new TripDriverDocumentDto
        {
            Type = type,
            Paths = paths,
            RecordingId = recordingId,
            WaypointId = waypointId,
            CreatedAt = createdAt
        });
    }
}
