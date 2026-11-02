using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.API.Resources.Admin.Trips;

public class TripTrackItemResource
{
    public long Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public string DriverMobile { get; set; } = string.Empty;
    public string VehiclePlateNumber { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
    public string VehicleColor { get; set; } = string.Empty;
    public TripLocationResource? CurrentLocation { get; set; }
    public List<TripWaypointResource>? Waypoints { get; set; }

    public static TripTrackItemResource FromDto(TripTrackItemDto dto, string preferredLanguage = "en") =>
        new()
        {
            Id = dto.Id,
            ReferenceNumber = TripReferenceNumber.Format(dto.Id),
            CompanyName = preferredLanguage == "ar" ? dto.CompanyNameAr : dto.CompanyNameEn,
            DriverName = dto.DriverName,
            DriverMobile = dto.DriverMobile,
            VehiclePlateNumber = dto.VehiclePlateNumber,
            VehicleType = dto.VehicleType,
            VehicleColor = dto.VehicleColor,
            Waypoints = dto.Waypoints?.Select(TripWaypointResource.FromDto).ToList(),
            CurrentLocation = dto.CurrentLocation != null
                ? new TripLocationResource
                {
                    Latitude = dto.CurrentLocation.Latitude,
                    Longitude = dto.CurrentLocation.Longitude,
                    RecordedAt = dto.CurrentLocation.RecordedAt
                }
                : null
        };
}
