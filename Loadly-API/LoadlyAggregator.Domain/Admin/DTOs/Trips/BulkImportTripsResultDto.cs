namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class BulkImportTripsResultDto
{
    public int Created { get; set; }
    public int Updated { get; set; }
    public int SkippedExisting { get; set; }
    public int InvalidRows { get; set; }
    public List<string> ExistingIdentifiers { get; set; } = new();
    public List<string> InvalidRowReasons { get; set; } = new();
}
