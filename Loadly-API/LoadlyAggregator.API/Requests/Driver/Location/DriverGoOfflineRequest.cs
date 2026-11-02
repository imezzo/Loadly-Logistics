using LoadlyAggregator.Domain.Driver.DTOs.Location;

namespace LoadlyAggregator.API.Requests.Driver.Location;

public class DriverGoOfflineRequest
{
    public DriverGoOfflineDto FromRequest(long driverId) => new()
    {
        DriverId = driverId
    };
}
