using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Company.DTOs.Contracts;

namespace LoadlyAggregator.API.Requests.Company.Contracts;

public class CompanyGetContractRequest
{
    public const string Permission = "view_contract";

    public long Id { get; set; }

    public CompanyGetContractDto FromRequest(long companyId) => new()
    {
        Id = Id,
        CompanyId = companyId
    };
}

public class CompanyGetContractRequestValidator : AbstractValidator<CompanyGetContractRequest>
{
    public CompanyGetContractRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Contract ID");
    }
}
