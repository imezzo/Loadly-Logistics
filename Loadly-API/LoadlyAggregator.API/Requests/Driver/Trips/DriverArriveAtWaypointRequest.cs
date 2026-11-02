using FluentValidation;
using LoadlyAggregator.Domain.Driver.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Driver.Trips;

public class DriverArriveAtWaypointRequest
{
    public DriverArriveAtWaypointDto FromRequest(long tripId, long waypointId, long driverId) => new()
    {
        TripId = tripId,
        WaypointId = waypointId,
        DriverId = driverId
    };
}

public class DriverArriveAtWaypointRequestValidator : AbstractValidator<DriverArriveAtWaypointRequest>
{
    public DriverArriveAtWaypointRequestValidator()
    {
        // No body fields to validate; extend when Note or other fields are added.
    }
}
