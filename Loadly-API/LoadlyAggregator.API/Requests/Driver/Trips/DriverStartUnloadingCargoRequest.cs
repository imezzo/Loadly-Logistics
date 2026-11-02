using FluentValidation;
using LoadlyAggregator.Domain.Driver.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Driver.Trips;

public class DriverStartUnloadingCargoRequest
{
    public DriverStartUnloadingCargoDto FromRequest(long tripId, long waypointId, long driverId) => new()
    {
        TripId = tripId,
        WaypointId = waypointId,
        DriverId = driverId
    };
}

public class DriverStartUnloadingCargoRequestValidator : AbstractValidator<DriverStartUnloadingCargoRequest>
{
    public DriverStartUnloadingCargoRequestValidator()
    {
        // No body fields to validate
    }
}
