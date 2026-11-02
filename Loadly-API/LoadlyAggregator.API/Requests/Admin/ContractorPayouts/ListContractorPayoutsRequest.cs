using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.ContractorPayouts;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.ContractorPayouts;

public class ListContractorPayoutsRequest : BasePagedRequest
{
    public const string Permission = GetContractorPayoutRequest.Permission;

    public long? ContractorId { get; set; }
    public ContractorPayoutStatus? Status { get; set; }

    public ListContractorPayoutsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        ContractorId = ContractorId,
        Status = Status
    };
}

public class ListContractorPayoutsRequestValidator : AbstractValidator<ListContractorPayoutsRequest>
{
    public ListContractorPayoutsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
        RuleFor(x => x.ContractorId)
            .IsOptionalId("Contractor ID")
            .When(x => x.ContractorId.HasValue);
        RuleFor(x => x.Status)
            .IsValidEnum<ListContractorPayoutsRequest, ContractorPayoutStatus>("Status")
            .When(x => x.Status.HasValue);
    }
}
