using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Company.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Company.Trips;

public class CompanyCancelTripRequest
{
    public const string Permission = "delete_trip";

    public string? Reason { get; set; }

    public CompanyCancelTripDto FromRequest(long tripId, long companyId) => new()
    {
        TripId = tripId,
        CompanyId = companyId,
        Reason = Reason
    };
}

public class CompanyCancelTripRequestValidator : AbstractValidator<CompanyCancelTripRequest>
{
    public CompanyCancelTripRequestValidator()
    {
        When(x => !string.IsNullOrWhiteSpace(x.Reason), () =>
            RuleFor(x => x.Reason).MaximumLength(500).WithMessage("validation.cancel_reason_max_length|500"));
    }
}
