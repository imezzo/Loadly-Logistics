using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;

public class ExitsCustomsDto
{
    public long TripId { get; set; }
    public decimal? CustomsTotalAmount { get; set; }
    public string? Note { get; set; }
    public List<FileUpload>? CustomsReceipts { get; set; }
    public DriverLocationData? CurrentLocation { get; set; }
}
