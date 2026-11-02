using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Driver.DTOs.Trips;

public class DriverCompleteTripDto
{
    public long TripId { get; set; }
    public long DriverId { get; set; }
    public int Odometer { get; set; }
    public decimal? TollgatesTotalAmount { get; set; }
    public decimal? TollgatesWeightTotalAmount { get; set; }
    public FileUpload? OdometerImage { get; set; }
    public string? Note { get; set; }
    public List<FileUpload>? TollgateImages { get; set; }
    public List<FileUpload>? TollgateWeightImages { get; set; }
    public FileUpload? DeliveryNoteImage { get; set; }
    public DriverLocationData? CurrentLocation { get; set; }
}
