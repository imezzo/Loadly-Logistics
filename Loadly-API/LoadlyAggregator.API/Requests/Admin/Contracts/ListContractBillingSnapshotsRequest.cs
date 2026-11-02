using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class ListContractBillingSnapshotsRequest : BasePagedRequest
{
    public const string Permission = GetContractBillingSnapshotRequest.Permission;

    public long? ContractId { get; set; }
    public ContractBillingSnapshotStatus? Status { get; set; }
    public DateOnly? PeriodStart { get; set; }
    public DateOnly? PeriodEnd { get; set; }

    public ListContractBillingSnapshotsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        ContractId = ContractId,
        Status = Status,
        PeriodStart = PeriodStart,
        PeriodEnd = PeriodEnd
    };
}

public class ListContractBillingSnapshotsRequestValidator
    : AbstractValidator<ListContractBillingSnapshotsRequest>
{
    public ListContractBillingSnapshotsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
        RuleFor(x => x.Search).IsOptionalString(255, "Search term");
        RuleFor(x => x.ContractId).IsOptionalId("Contract ID");
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("validation.invalid_named_value|Status")
            .When(x => x.Status.HasValue);
        RuleFor(x => x.PeriodEnd)
            .GreaterThanOrEqualTo(x => x.PeriodStart!.Value)
            .WithMessage("validation.end_date_gte_start_date")
            .When(x => x.PeriodStart.HasValue && x.PeriodEnd.HasValue);
    }
}
