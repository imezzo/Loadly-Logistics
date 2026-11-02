using FluentValidation;
using LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;

namespace LoadlyAggregator.API.Requests.Admin.TripsManagement;

public class ArriveAndStartUnloadingRequest
{
    public const string Permission = "self_complete_trip";

    public ArriveAndStartUnloadingDto FromRequest(long tripId, long waypointId) => new()
    {
        TripId = tripId,
        WaypointId = waypointId
    };
}

public class ArriveAndStartUnloadingRequestValidator : AbstractValidator<ArriveAndStartUnloadingRequest>
{
    public ArriveAndStartUnloadingRequestValidator()
    {
    }
}
