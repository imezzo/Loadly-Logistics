namespace LoadlyAggregator.Shared.DTOs.Common;

public class PaginationDto
{
    /// <summary>Total item count when known; null when using single-query (no count) pagination.</summary>
    public int? TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }

    public PaginationDto(int page, int pageSize, int totalCount)
    {
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}
