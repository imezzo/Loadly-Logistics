using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.Actions.Contracts;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class PatchContractBillingSnapshotCommercialTermsRequest
{
    public const string Permission = UpdateDraftContractBillingSnapshotRequest.Permission;

    public List<long> SeatRootSlotIds { get; set; } = [];
    public DateOnly EffectiveStart { get; set; }
    public DateOnly EffectiveEnd { get; set; }
    public decimal? MonthlyRental { get; set; }
    public decimal? ExtraWorkingDayFees { get; set; }
    public decimal? OverConsumption { get; set; }
    public string? Reason { get; set; }

    public PatchContractBillingSnapshotCommercialTermsDto FromRequest(long snapshotId) => new()
    {
        SnapshotId = snapshotId,
        SeatRootSlotIds = SeatRootSlotIds ?? [],
        EffectiveStart = EffectiveStart,
        EffectiveEnd = EffectiveEnd,
        MonthlyRental = MonthlyRental,
        ExtraWorkingDayFees = ExtraWorkingDayFees,
        OverConsumption = OverConsumption,
        Reason = Reason
    };
}

public class PatchContractBillingSnapshotCommercialTermsRequestValidator
    : AbstractValidator<PatchContractBillingSnapshotCommercialTermsRequest>
{
    public PatchContractBillingSnapshotCommercialTermsRequestValidator()
    {
        RuleFor(x => x.SeatRootSlotIds)
            .NotEmpty()
            .WithMessage("contract_billing_snapshot.commercial_override_seats_required");
        RuleForEach(x => x.SeatRootSlotIds).IsGreaterThanZero("Seat root slot ID");
        RuleFor(x => x.EffectiveEnd)
            .GreaterThanOrEqualTo(x => x.EffectiveStart)
            .WithMessage("contract_billing_snapshot.commercial_override_invalid_range");
        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("contract_billing_snapshot.commercial_override_reason_required")
            .MaximumLength(500);
        RuleFor(x => x)
            .Must(x => x.MonthlyRental.HasValue || x.ExtraWorkingDayFees.HasValue || x.OverConsumption.HasValue)
            .WithMessage("contract_billing_snapshot.commercial_override_rate_required");
        RuleFor(x => x.MonthlyRental)
            .GreaterThan(0)
            .When(x => x.MonthlyRental.HasValue);
        RuleFor(x => x.ExtraWorkingDayFees)
            .GreaterThan(0)
            .When(x => x.ExtraWorkingDayFees.HasValue);
        RuleFor(x => x.OverConsumption)
            .GreaterThan(0)
            .When(x => x.OverConsumption.HasValue);
    }
}

public class DeleteContractBillingSnapshotCommercialTermsRequest
{
    public const string Permission = PatchContractBillingSnapshotCommercialTermsRequest.Permission;

    public long SeatRootSlotId { get; set; }

    public DeleteContractBillingSnapshotCommercialTermsDto FromRequest(long snapshotId) => new()
    {
        SnapshotId = snapshotId,
        SeatRootSlotId = SeatRootSlotId
    };
}

public class DeleteContractBillingSnapshotCommercialTermsRequestValidator
    : AbstractValidator<DeleteContractBillingSnapshotCommercialTermsRequest>
{
    public DeleteContractBillingSnapshotCommercialTermsRequestValidator()
    {
        RuleFor(x => x.SeatRootSlotId).IsGreaterThanZero("Seat root slot ID");
    }
}
