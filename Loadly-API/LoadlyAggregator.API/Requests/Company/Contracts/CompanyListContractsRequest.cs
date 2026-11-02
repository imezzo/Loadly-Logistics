using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Company.DTOs.Contracts;

namespace LoadlyAggregator.API.Requests.Company.Contracts;

public class CompanyListContractsRequest : BasePagedRequest
{
    public const string Permission = "view_contract";

    public long? ContractId { get; set; }
    public ContractType? ContractType { get; set; }
    public string? ReferenceNumber { get; set; }

    public CompanyListContractsDto FromRequest(long companyId) => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        CompanyId = companyId,
        ContractId = ContractId,
        ContractType = ContractType,
        ReferenceNumber = ReferenceNumber
    };
}

public class CompanyListContractsRequestValidator : AbstractValidator<CompanyListContractsRequest>
{
    public CompanyListContractsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
        RuleFor(x => x.Search).IsOptionalString(255, "Search term");
        RuleFor(x => x.ContractId).IsOptionalId("Contract ID");
        RuleFor(x => x.ContractType)
            .IsInEnum().WithMessage("validation.invalid_contract_type")
            .When(x => x.ContractType.HasValue);
    }
}
