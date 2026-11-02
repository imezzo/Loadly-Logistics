using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Auth;
using LoadlyAggregator.Domain.Admin.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Admin.Auth;

public class AdminListSessionsRequest
{
    public long UserId { get; set; }
    public string? CurrentTokenFamilyId { get; set; }

    public AdminListSessionsDto FromRequest() => new()
    {
        UserId = UserId,
        CurrentTokenFamilyId = CurrentTokenFamilyId
    };
}

public class AdminListSessionsRequestValidator : AbstractValidator<AdminListSessionsRequest>
{
    public AdminListSessionsRequestValidator()
    {

    }
}
