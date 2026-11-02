using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Collector.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Collector.Trips;

public class BulkConfirmTripsDocumentsRequest
{
    public const string Permission = "confirm_trip_documents";

    public List<long> TripIds { get; set; } = new();

    public BulkConfirmTripsDocumentsDto FromRequest(long financeCollectorId) => new()
    {
        TripIds = TripIds,
        FinanceCollectorId = financeCollectorId
    };
}

public class BulkConfirmTripsDocumentsRequestValidator : AbstractValidator<BulkConfirmTripsDocumentsRequest>
{
    public BulkConfirmTripsDocumentsRequestValidator()
    {
        RuleFor(x => x.TripIds)
            .IsRequiredNonEmptyCollection("TripIds");

        RuleForEach(x => x.TripIds)
            .IsRequiredId("Trip ID");
    }
}
