using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;

public class EntersCustomsDto
{
    public long TripId { get; set; }
    public string? Note { get; set; }
    public List<FileUpload>? CustomsDocuments { get; set; }
    public DriverLocationData? CurrentLocation { get; set; }
}
