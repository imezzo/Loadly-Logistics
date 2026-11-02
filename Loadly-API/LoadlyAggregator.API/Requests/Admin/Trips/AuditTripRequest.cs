using FluentValidation;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Trips;

public class AuditTripRequest
{
    public const string Permission = "audit_trip";

    public string? ProofOfDelivery { get; set; }
    public bool ConfirmFinanceApproval { get; set; }
    public DateOnly FinancialDate { get; set; }
    public List<UpdateTripChargeItemRequest> TripCharges { get; set; } = [];

    public AuditTripDto FromRequest(long id) => new()
    {
        Id = id,
        ProofOfDelivery = ProofOfDelivery,
        ConfirmFinanceApproval = ConfirmFinanceApproval,
        FinancialDate = FinancialDate,
        TripCharges = TripCharges
            .Select(c => new UpdateTripChargeItemDto
            {
                Id = c.Id ?? 0,
                ChargeType = c.ChargeType,
                ExtraFeeTypeId = c.ExtraFeeTypeId,
                Amount = c.Amount,
                CompanyShare = c.CompanyShare
            })
            .ToList()
    };
}

public class AuditTripRequestValidator : AbstractValidator<AuditTripRequest>
{
    public AuditTripRequestValidator()
    {
        RuleFor(x => x.FinancialDate)
            .NotEmpty().WithMessage("validation.required");

        When(x => x.ProofOfDelivery != null, () =>
        {
            RuleFor(x => x.ProofOfDelivery!)
                .NotEmpty().WithMessage("validation.required");
        });

        RuleFor(x => x)
            .Must(x => !x.ConfirmFinanceApproval || !string.IsNullOrWhiteSpace(x.ProofOfDelivery))
            .WithMessage(TripMessageKeys.ConfirmFinanceApprovalRequiresProofOfDelivery);

        RuleFor(x => x.TripCharges)
            .NotNull().WithMessage(TripMessageKeys.ListCannotBeEmpty)
            .Must(list => list != null && list.Count > 0).WithMessage(TripMessageKeys.ListCannotBeEmpty);

        When(x => x.TripCharges is { Count: > 0 }, () =>
        {
            RuleFor(x => x.TripCharges)
                .Must(list => list.Where(c => c.Id is > 0).Select(c => c.Id!.Value).Distinct().Count() == list.Count(c => c.Id is > 0))
                .WithMessage("validation.trip_charge_line_ids_unique");

            RuleFor(x => x.TripCharges)
                .Must(list => list.Select(c => c.ChargeType).Distinct().Count() == list.Count)
                .WithMessage(TripMessageKeys.DuplicateTripChargeTypeInUpdateRequest);

            RuleForEach(x => x.TripCharges).ChildRules(charge =>
            {
                charge.RuleFor(c => c.Id)
                    .Must(id => !id.HasValue || id.Value > 0)
                    .WithMessage("validation.trip_charge_id_omit_or_gt_zero");
                charge.RuleFor(c => c.ChargeType)
                    .Must(t => Enum.IsDefined(typeof(TripChargeType), t))
                    .WithMessage("validation.trip_charge_type_invalid");
                charge.RuleFor(c => c.ExtraFeeTypeId)
                    .Must(id => !id.HasValue || Enum.IsDefined(typeof(ExtraFeeTypes), id.Value))
                    .WithMessage("validation.trip_extra_fee_type_invalid");
                charge.RuleFor(c => c.Amount)
                    .GreaterThanOrEqualTo(0).WithMessage("validation.must_be_greater_than_or_equal_to_zero");
                charge.RuleFor(c => c.CompanyShare)
                    .GreaterThanOrEqualTo(0).When(c => c.CompanyShare.HasValue)
                    .WithMessage("validation.must_be_greater_than_or_equal_to_zero");
                charge.RuleFor(c => c.CompanyShare)
                    .LessThanOrEqualTo(c => c.Amount).When(c => c.CompanyShare.HasValue)
                    .WithMessage("validation.company_share_must_not_exceed_amount");
            });
        });
    }
}
