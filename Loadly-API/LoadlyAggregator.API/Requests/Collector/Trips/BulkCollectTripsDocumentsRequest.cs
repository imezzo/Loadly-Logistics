using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Collector.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Collector.Trips;

public class BulkCollectTripsDocumentsRequest
{
    public const string Permission = CollectTripDocumentsRequest.Permission;

    public List<long> TripIds { get; set; } = new();

    public BulkCollectTripsDocumentsDto FromRequest(long collectorId) => new()
    {
        TripIds = TripIds,
        CollectorId = collectorId
    };
}

public class BulkCollectTripsDocumentsRequestValidator : AbstractValidator<BulkCollectTripsDocumentsRequest>
{
    public BulkCollectTripsDocumentsRequestValidator()
    {
        RuleFor(x => x.TripIds)
            .IsRequiredNonEmptyCollection("TripIds");

        RuleForEach(x => x.TripIds)
            .IsRequiredId("Trip ID");
    }
}
