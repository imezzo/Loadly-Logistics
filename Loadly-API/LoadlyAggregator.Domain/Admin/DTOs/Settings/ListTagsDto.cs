using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class ListTagsDto : BasePagedRequest
{
    public bool? IsPricing { get; set; }
}
