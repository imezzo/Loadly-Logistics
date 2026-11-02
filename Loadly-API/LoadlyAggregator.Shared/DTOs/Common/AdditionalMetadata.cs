namespace LoadlyAggregator.Shared.DTOs.Common;

public class AdditionalMetadata
{
    public List<ExportKey>? ExportKeys { get; set; }
}

public class ExportKey
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}
