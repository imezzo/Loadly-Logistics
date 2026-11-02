using FluentValidation;
using LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;

namespace LoadlyAggregator.API.Requests.Admin.TripsManagement;

public class StartUnloadingCargoRequest
{
    public const string Permission = "self_complete_trip";

    public StartUnloadingCargoDto FromRequest(long tripId, long waypointId) => new()
    {
        TripId = tripId,
        WaypointId = waypointId
    };
}

public class StartUnloadingCargoRequestValidator : AbstractValidator<StartUnloadingCargoRequest>
{
    public StartUnloadingCargoRequestValidator()
    {
    }
}
