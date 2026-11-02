using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.ContractorPayouts;

namespace LoadlyAggregator.API.Requests.Admin.ContractorPayouts;

public class BulkSetContractorPayoutsPaidRequest
{
    public const string Permission = SetContractorPayoutPaidRequest.Permission;

    public List<long> PayoutIds { get; set; } = [];

    public BulkSetContractorPayoutsPaidDto FromRequest() => new()
    {
        PayoutIds = PayoutIds
    };
}

public class BulkSetContractorPayoutsPaidRequestValidator : AbstractValidator<BulkSetContractorPayoutsPaidRequest>
{
    public BulkSetContractorPayoutsPaidRequestValidator()
    {
        RuleFor(x => x.PayoutIds)
            .IsRequiredNonEmptyCollection("PayoutIds");
        RuleForEach(x => x.PayoutIds)
            .IsRequiredId("Payout ID");
    }
}
