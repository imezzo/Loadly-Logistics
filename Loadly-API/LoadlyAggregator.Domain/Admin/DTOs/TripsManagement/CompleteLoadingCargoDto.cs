using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;

public class CompleteLoadingCargoDto
{
    public long TripId { get; set; }
    public long WaypointId { get; set; }
    public string? Note { get; set; }
    public FileUpload? ReceiptNoteImage { get; set; }
    public List<FileUpload>? ShipmentImages { get; set; }
    public DriverLocationData? CurrentLocation { get; set; }
}
