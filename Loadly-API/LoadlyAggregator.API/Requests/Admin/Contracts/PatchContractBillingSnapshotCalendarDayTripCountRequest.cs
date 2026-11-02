using FluentValidation;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class PatchContractBillingSnapshotCalendarDayTripCountRequest
{
    public const string Permission = PatchContractBillingSnapshotCalendarDayRequest.Permission;

    public List<long>? TripIds { get; set; }
    public string? Reason { get; set; }

    public PatchContractBillingSnapshotCalendarDayTripCountDto FromRequest(long snapshotId, long calendarDayId) => new()
    {
        SnapshotId = snapshotId,
        CalendarDayId = calendarDayId,
        TripIds = TripIds ?? [],
        Reason = Reason
    };
}

public class PatchContractBillingSnapshotCalendarDayTripCountRequestValidator
    : AbstractValidator<PatchContractBillingSnapshotCalendarDayTripCountRequest>
{
    public PatchContractBillingSnapshotCalendarDayTripCountRequestValidator()
    {
        RuleFor(x => x.TripIds)
            .NotNull()
            .WithMessage("validation.required");
        RuleForEach(x => x.TripIds)
            .GreaterThan(0)
            .WithMessage("validation.must_be_greater_than_zero");
        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .WithMessage("validation.must_not_exceed_characters|500")
            .When(x => !string.IsNullOrWhiteSpace(x.Reason));
    }
}
