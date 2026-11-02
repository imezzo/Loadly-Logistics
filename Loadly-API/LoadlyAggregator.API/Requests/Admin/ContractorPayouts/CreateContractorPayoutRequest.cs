using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.ContractorPayouts;

namespace LoadlyAggregator.API.Requests.Admin.ContractorPayouts;

public class CreateContractorPayoutRequest
{
    public const string Permission = "create_contractor_payout";

    public List<long> TripIds { get; set; } = [];

    public CreateContractorPayoutDto FromRequest() => new()
    {
        TripIds = TripIds
    };
}

public class CreateContractorPayoutRequestValidator : AbstractValidator<CreateContractorPayoutRequest>
{
    public CreateContractorPayoutRequestValidator()
    {
        RuleFor(x => x.TripIds)
            .IsRequiredNonEmptyCollection("TripIds");
        RuleForEach(x => x.TripIds)
            .IsRequiredId("Trip ID");
    }
}
