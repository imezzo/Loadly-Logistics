using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class SetTripDispatchingModeDto
{
    public long Id { get; set; }
    public TripDispatchingMode DispatchingMode { get; set; }
}
