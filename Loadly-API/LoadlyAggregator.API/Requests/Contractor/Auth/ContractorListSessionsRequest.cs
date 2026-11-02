using FluentValidation;
using LoadlyAggregator.API.Requests.Contractor.Auth;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Contractor.Auth;

public class ContractorListSessionsRequest
{
    public long UserId { get; set; }
    public string? CurrentTokenFamilyId { get; set; }

    public ContractorListSessionsDto FromRequest() => new()
    {
        UserId = UserId,
        CurrentTokenFamilyId = CurrentTokenFamilyId
    };
}

public class ContractorListSessionsRequestValidator : AbstractValidator<ContractorListSessionsRequest>
{
    public ContractorListSessionsRequestValidator()
    {

    }
}
