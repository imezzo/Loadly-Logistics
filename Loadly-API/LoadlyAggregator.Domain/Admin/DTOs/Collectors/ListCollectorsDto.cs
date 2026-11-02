using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.DTOs.Collectors;

public class ListCollectorsDto : BasePagedRequest
{
    public int? Status { get; set; }
}
