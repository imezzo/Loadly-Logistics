using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Company.DTOs.Contracts;

namespace LoadlyAggregator.API.Requests.Company.Contracts;

public class CompanyGetContractDestinationRequest
{
    public const string Permission = CompanyGetContractRequest.Permission;

    public long DestinationId { get; set; }

    public CompanyGetContractDestinationDto FromRequest(long companyId) => new()
    {
        Id = DestinationId,
        CompanyId = companyId
    };
}

public class CompanyGetContractDestinationRequestValidator : AbstractValidator<CompanyGetContractDestinationRequest>
{
    public CompanyGetContractDestinationRequestValidator()
    {
        RuleFor(x => x.DestinationId).IsRequiredId("Destination ID");
    }
}
