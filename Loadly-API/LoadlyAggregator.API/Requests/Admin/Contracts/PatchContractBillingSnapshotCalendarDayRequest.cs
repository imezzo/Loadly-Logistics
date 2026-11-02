using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.Actions.Contracts;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class PatchContractBillingSnapshotCalendarDayRequest
{
    public const string Permission = UpdateDraftContractBillingSnapshotRequest.Permission;

    public ContractSlotDayDisposition Disposition { get; set; }
    public long? FacilityId { get; set; }
    public string? Reason { get; set; }

    public PatchContractBillingSnapshotCalendarDayDto FromRequest(long snapshotId, long calendarDayId) => new()
    {
        SnapshotId = snapshotId,
        CalendarDayId = calendarDayId,
        Disposition = Disposition,
        FacilityId = FacilityId,
        Reason = Reason
    };
}

public class PatchContractBillingSnapshotCalendarDayRequestValidator
    : AbstractValidator<PatchContractBillingSnapshotCalendarDayRequest>
{
    public PatchContractBillingSnapshotCalendarDayRequestValidator()
    {
        RuleFor(x => x.Disposition)
            .IsInEnum()
            .WithMessage("validation.invalid_enum|Disposition");
        RuleFor(x => x.FacilityId)
            .NotNull()
            .WithMessage("contract_billing_snapshot.facility_required_for_out_of_service")
            .When(x => x.Disposition == ContractSlotDayDisposition.Skipped);
        RuleFor(x => x.FacilityId).IsOptionalId("Facility ID");
        RuleFor(x => x.FacilityId)
            .Null()
            .WithMessage("contract_billing_snapshot.facility_only_for_out_of_service_or_working")
            .When(x => x.Disposition is not ContractSlotDayDisposition.Skipped
                and not ContractSlotDayDisposition.Working
                and not ContractSlotDayDisposition.HalfWorking);
        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Reason));
    }
}
