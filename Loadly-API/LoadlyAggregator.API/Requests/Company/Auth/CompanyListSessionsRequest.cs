using FluentValidation;
using LoadlyAggregator.API.Requests.Company.Auth;
using LoadlyAggregator.Domain.Company.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Company.Auth;

public class CompanyListSessionsRequest
{
    public long UserId { get; set; }
    public string? CurrentTokenFamilyId { get; set; }

    public CompanyListSessionsDto FromRequest() => new()
    {
        UserId = UserId,
        CurrentTokenFamilyId = CurrentTokenFamilyId
    };
}

public class CompanyListSessionsRequestValidator : AbstractValidator<CompanyListSessionsRequest>
{
    public CompanyListSessionsRequestValidator()
    {

    }
}
