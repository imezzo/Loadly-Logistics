using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Resources.Admin.Vehicles;

public class BulkImportVehiclesResultResource
{
    public int Created { get; set; }
    public int Updated { get; set; }
    public int SkippedExisting { get; set; }
    public int InvalidRows { get; set; }
    public List<string> ExistingIdentifiers { get; set; } = new();
    public List<string> InvalidRowReasons { get; set; } = new();

    public static BulkImportVehiclesResultResource FromResult(BulkImportVehiclesResultDto dto) => new()
    {
        Created = dto.Created,
        Updated = dto.Updated,
        SkippedExisting = dto.SkippedExisting,
        InvalidRows = dto.InvalidRows,
        ExistingIdentifiers = dto.ExistingIdentifiers ?? new List<string>(),
        InvalidRowReasons = dto.InvalidRowReasons ?? new List<string>()
    };
}
