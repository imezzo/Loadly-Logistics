using FluentValidation;
using LoadlyAggregator.Domain.Driver.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Driver.Trips;

public class DriverStartLoadingCargoRequest
{
    public DriverStartLoadingCargoDto FromRequest(long tripId, long waypointId, long driverId) => new()
    {
        TripId = tripId,
        WaypointId = waypointId,
        DriverId = driverId
    };
}

public class DriverStartLoadingCargoRequestValidator : AbstractValidator<DriverStartLoadingCargoRequest>
{
    public DriverStartLoadingCargoRequestValidator()
    {
        // No body fields to validate
    }
}
