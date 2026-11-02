namespace LoadlyAggregator.Domain.Common.DTOs;

/// <summary>
/// Metadata for a single calculation option (e.g. TollGate Fixed, HelpersFee PerHour).
/// Used by GET /api/lookups/extra-fee-types for dynamic form rendering.
/// </summary>
public class ExtraFeeOptionMetaDto
{
    public int OptionEnum { get; set; }
    public string Label { get; set; } = string.Empty;
    public bool RequiresValue { get; set; }
    public string? ValueType { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Metadata for an extra fee type (1–7) including names, threshold requirements, and options.
/// </summary>
public class ExtraFeeTypeMetaDto
{
    public int TypeId { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public bool RequiresThreshold { get; set; }
    public string? ThresholdLabel { get; set; }
    public string? ThresholdValuePlaceHolder { get; set; }
    public string? ThresholdDescription { get; set; }
    public List<ExtraFeeOptionMetaDto> Options { get; set; } = new();
}
