using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Collector.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Collector.Trips;

public class CollectorGetTripRequest
{
    public const string Permission = CollectTripDocumentsRequest.Permission;

    public long Id { get; set; }

    public CollectorGetTripDto FromRequest() => new() { Id = Id };
}

public class CollectorGetTripRequestValidator : AbstractValidator<CollectorGetTripRequest>
{
    public CollectorGetTripRequestValidator()
    {
        RuleFor(x => x.Id)
            .IsRequiredId("Trip ID");
    }
}
