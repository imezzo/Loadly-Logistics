namespace LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

public class BulkImportVehiclesResultDto
{
    public int Created { get; set; }
    public int Updated { get; set; }
    public int SkippedExisting { get; set; }
    public int InvalidRows { get; set; }
    public List<string> ExistingIdentifiers { get; set; } = new();
    public List<string> InvalidRowReasons { get; set; } = new();
}
