using FluentValidation;
using LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;

namespace LoadlyAggregator.API.Requests.Admin.TripsManagement;

public class ArriveAndStartLoadingRequest
{
    public const string Permission = "self_complete_trip";

    public ArriveAndStartLoadingDto FromRequest(long tripId, long waypointId) => new()
    {
        TripId = tripId,
        WaypointId = waypointId
    };
}

public class ArriveAndStartLoadingRequestValidator : AbstractValidator<ArriveAndStartLoadingRequest>
{
    public ArriveAndStartLoadingRequestValidator()
    {
    }
}
