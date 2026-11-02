using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class SetTripOverNightCostBearerDto
{
    public long Id { get; set; }
    public OverNightCostBearer OverNightCostBearer { get; set; }
}
