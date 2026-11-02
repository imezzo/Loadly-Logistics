namespace LoadlyAggregator.Shared.DTOs.Common;

public class LookupResponse
{
    public List<LookupItem> Data { get; set; } = new();
    public PaginationDto Pagination { get; set; } = null!;
}
