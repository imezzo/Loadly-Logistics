using FluentValidation;
using LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;

namespace LoadlyAggregator.API.Requests.Admin.TripsManagement;

public class StartLoadingCargoRequest
{
    public const string Permission = "self_complete_trip";

    public StartLoadingCargoDto FromRequest(long tripId, long waypointId) => new()
    {
        TripId = tripId,
        WaypointId = waypointId
    };
}

public class StartLoadingCargoRequestValidator : AbstractValidator<StartLoadingCargoRequest>
{
    public StartLoadingCargoRequestValidator()
    {
    }
}
