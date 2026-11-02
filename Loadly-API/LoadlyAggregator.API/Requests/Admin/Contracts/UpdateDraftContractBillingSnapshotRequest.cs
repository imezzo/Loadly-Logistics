using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Helpers;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class UpdateDraftContractBillingSnapshotRequest
{
    public const string Permission = "edit_contract_billing_snapshot";

    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public long? ContractSlotId { get; set; }
    public string? Name { get; set; }

    public UpdateDraftContractBillingSnapshotDto FromRequest(long contractId, long snapshotId) => new()
    {
        ContractId = contractId,
        SnapshotId = snapshotId,
        ContractSlotId = ContractSlotId,
        PeriodStart = PeriodStart,
        PeriodEnd = PeriodEnd,
        Name = Name
    };
}

public class UpdateDraftContractBillingSnapshotRequestValidator
    : AbstractValidator<UpdateDraftContractBillingSnapshotRequest>
{
    public UpdateDraftContractBillingSnapshotRequestValidator()
    {
        RuleFor(x => x.PeriodStart).Must(date => date != default).WithMessage("validation.field_is_required|Period start");
        RuleFor(x => x.PeriodEnd).Must(date => date != default).WithMessage("validation.field_is_required|Period end");
        RuleFor(x => x.PeriodEnd)
            .GreaterThanOrEqualTo(x => x.PeriodStart)
            .WithMessage("validation.end_date_gte_start_date");
        RuleFor(x => x.ContractSlotId).IsOptionalId("Contract slot");
        RuleFor(x => x.Name)
            .MaximumLength(ContractBillingSnapshotNameFormatter.MaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Name));
    }
}
