using FluentValidation;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Admin.Trips;

public class SummarizeTripChargesByInvoiceTypeRequest
{
    public const string Permission = "view_trip";

    public List<long> TripIds { get; set; } = [];

    public SummarizeTripChargesByInvoiceTypeDto FromRequest() => new()
    {
        TripIds = TripIds
    };
}

public class SummarizeTripChargesByInvoiceTypeRequestValidator
    : AbstractValidator<SummarizeTripChargesByInvoiceTypeRequest>
{
    public SummarizeTripChargesByInvoiceTypeRequestValidator()
    {
        RuleFor(x => x.TripIds)
            .NotNull().WithMessage("trips.list_cannot_be_empty")
            .Must(ids => ids is { Count: > 0 }).WithMessage("trips.list_cannot_be_empty");

        RuleForEach(x => x.TripIds)
            .GreaterThan(0).WithMessage("validation.must_be_greater_than_zero");
    }
}
