using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Contracts;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class ListContractsRequest : BasePagedRequest
{
    public const string Permission = "view_contract";

    public long? ContractId { get; set; }
    public long? CompanyId { get; set; }
    public ContractType? ContractType { get; set; }
    public long? SalesRepId { get; set; }
    public ContractStatus? Status { get; set; }
    public string? ReferenceNumber { get; set; }

    public ListContractsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        ContractId = ContractId,
        CompanyId = CompanyId,
        ContractType = ContractType,
        SalesRepId = SalesRepId,
        Status = Status,
        ReferenceNumber = ReferenceNumber
    };
}

public class ListContractsRequestValidator : AbstractValidator<ListContractsRequest>
{
    public ListContractsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
        RuleFor(x => x.Search).IsOptionalString(255, "Search term");
        RuleFor(x => x.ContractId)
            .IsOptionalId("Contract ID");
        RuleFor(x => x.CompanyId)
            .IsOptionalId("Company ID");
        RuleFor(x => x.ContractType)
            .IsInEnum().WithMessage("validation.invalid_contract_type")
            .When(x => x.ContractType.HasValue);
        RuleFor(x => x.SalesRepId)
            .IsOptionalId("Sales rep ID");
        // Status validation not needed - ContractStatus? enum handles valid values automatically
    }
}
