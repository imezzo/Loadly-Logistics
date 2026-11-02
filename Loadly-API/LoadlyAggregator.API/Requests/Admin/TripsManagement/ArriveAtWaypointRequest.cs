using FluentValidation;
using LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;

namespace LoadlyAggregator.API.Requests.Admin.TripsManagement;

public class ArriveAtWaypointRequest
{
    public const string Permission = "self_complete_trip";

    public ArriveAtWaypointDto FromRequest(long tripId, long waypointId) => new()
    {
        TripId = tripId,
        WaypointId = waypointId
    };
}

public class ArriveAtWaypointRequestValidator : AbstractValidator<ArriveAtWaypointRequest>
{
    public ArriveAtWaypointRequestValidator()
    {
    }
}
