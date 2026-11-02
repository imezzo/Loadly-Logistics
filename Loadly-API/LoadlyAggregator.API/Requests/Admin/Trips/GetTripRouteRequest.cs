using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Admin.Trips;

public class GetTripRouteRequest
{
    public const string Permission = "view_trip";

    public long TripId { get; set; }

    public GetTripRouteDto FromRequest() => new() { TripId = TripId };
}

public class GetTripRouteRequestValidator : AbstractValidator<GetTripRouteRequest>
{
    public GetTripRouteRequestValidator()
    {
        RuleFor(x => x.TripId)
            .IsRequiredId("Trip ID");
    }
}
