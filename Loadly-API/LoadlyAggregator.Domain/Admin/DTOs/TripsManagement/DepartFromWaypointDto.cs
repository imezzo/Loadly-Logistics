using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;

public class DepartFromWaypointDto
{
    public long TripId { get; set; }
    public long WaypointId { get; set; }
    public string? Note { get; set; }
    public FileUpload? DeliveryNoteImage { get; set; }
    public int? Odometer { get; set; }
    public FileUpload? OdometerImage { get; set; }
    public DriverLocationData? CurrentLocation { get; set; }
}
