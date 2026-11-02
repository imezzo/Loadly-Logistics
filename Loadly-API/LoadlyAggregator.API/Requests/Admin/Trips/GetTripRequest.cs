using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Trips;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Admin.Trips;

public class GetTripRequest
{
    public const string Permission = "view_trip";

    public long Id { get; set; }

    public GetTripDto FromRequest() => new() { Id = Id };
}

public class GetTripRequestValidator : AbstractValidator<GetTripRequest>
{
    public GetTripRequestValidator()
    {
        RuleFor(x => x.Id)
            .IsRequiredId("Trip ID");
    }
}
