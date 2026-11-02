using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Driver.DTOs.Trips;

public class DriverExitsCustomsDto
{
    public long TripId { get; set; }
    public long DriverId { get; set; }
    public decimal? CustomsTotalAmount { get; set; }
    public string? Note { get; set; }
    public List<FileUpload>? CustomsReceipts { get; set; }
    public DriverLocationData? CurrentLocation { get; set; }
}
