namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class TripTrackItemDto
{
    public long Id { get; set; }
    public string CompanyNameEn { get; set; } = string.Empty;
    public string CompanyNameAr { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public string DriverMobile { get; set; } = string.Empty;
    public string VehiclePlateNumber { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
    public string VehicleColor { get; set; } = string.Empty;
    public TripLocationDto? CurrentLocation { get; set; }
    public List<TripWaypointDto>? Waypoints { get; set; }
}
