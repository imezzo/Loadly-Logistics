namespace LoadlyAggregator.Shared.DTOs.Common;

public class ListingDto<T>
{
    public List<T> Data { get; set; } = new();
    public AdditionalMetadata? Additional { get; set; }
    public PaginationDto? Pagination { get; set; }
}
