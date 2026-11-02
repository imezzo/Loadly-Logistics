using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class ListDistrictsDto : BasePagedRequest
{
    public long? CityId { get; set; }
}
