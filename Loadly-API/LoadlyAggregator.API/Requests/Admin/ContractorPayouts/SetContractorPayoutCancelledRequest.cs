using FluentValidation;
using LoadlyAggregator.Domain.Admin.DTOs.ContractorPayouts;

namespace LoadlyAggregator.API.Requests.Admin.ContractorPayouts;

public class SetContractorPayoutCancelledRequest
{
    public const string Permission = "edit_contractor_payout";

    public string? CancellationReason { get; set; }

    public SetContractorPayoutCancelledDto FromRequest(long id) => new()
    {
        Id = id,
        CancellationReason = CancellationReason
    };
}

public class SetContractorPayoutCancelledRequestValidator : AbstractValidator<SetContractorPayoutCancelledRequest>
{
    public SetContractorPayoutCancelledRequestValidator()
    {
        When(x => !string.IsNullOrWhiteSpace(x.CancellationReason), () =>
            RuleFor(x => x.CancellationReason)
                .MaximumLength(500)
                .WithMessage("validation.cancellation_reason_max_length|500"));
    }
}
