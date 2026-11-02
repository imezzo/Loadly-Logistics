using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Trips;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Admin.Trips;

public class DeleteTripRequest
{
    public const string Permission = "delete_trip";

    public long Id { get; set; }

    public DeleteTripDto FromRequest() => new() { Id = Id };
}

public class DeleteTripRequestValidator : AbstractValidator<DeleteTripRequest>
{
    public DeleteTripRequestValidator()
    {
        RuleFor(x => x.Id)
            .IsRequiredId("Trip ID");
    }
}
