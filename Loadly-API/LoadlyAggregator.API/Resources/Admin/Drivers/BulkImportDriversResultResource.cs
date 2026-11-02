using LoadlyAggregator.Domain.Admin.DTOs.Drivers;

namespace LoadlyAggregator.API.Resources.Admin.Drivers;

public class BulkImportDriversResultResource
{
    public int Created { get; set; }
    public int Updated { get; set; }
    public int SkippedExisting { get; set; }
    public int InvalidRows { get; set; }
    public List<string> ExistingIdentifiers { get; set; } = new();
    public List<string> InvalidRowReasons { get; set; } = new();

    public static BulkImportDriversResultResource FromResult(BulkImportDriversResultDto dto) => new()
    {
        Created = dto.Created,
        Updated = dto.Updated,
        SkippedExisting = dto.SkippedExisting,
        InvalidRows = dto.InvalidRows,
        ExistingIdentifiers = dto.ExistingIdentifiers ?? new List<string>(),
        InvalidRowReasons = dto.InvalidRowReasons ?? new List<string>()
    };
}
