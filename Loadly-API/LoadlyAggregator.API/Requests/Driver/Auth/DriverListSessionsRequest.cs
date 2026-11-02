using FluentValidation;
using LoadlyAggregator.API.Requests.Driver.Auth;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Driver.Auth;

public class DriverListSessionsRequest
{
    public long UserId { get; set; }
    public string? CurrentTokenFamilyId { get; set; }

    public DriverListSessionsDto FromRequest() => new()
    {
        UserId = UserId,
        CurrentTokenFamilyId = CurrentTokenFamilyId
    };
}

public class DriverListSessionsRequestValidator : AbstractValidator<DriverListSessionsRequest>
{
    public DriverListSessionsRequestValidator()
    {

    }
}
