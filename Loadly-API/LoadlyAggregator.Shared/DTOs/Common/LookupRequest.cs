namespace LoadlyAggregator.Shared.DTOs.Common;

public class LookupRequest : BasePagedRequest
{
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; } = "asc";
    public int? Status { get; set; }
}
