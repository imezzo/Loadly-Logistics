using FluentValidation;
using LoadlyAggregator.API.Requests.Collector.Auth;
using LoadlyAggregator.Domain.Collector.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Collector.Auth;

public class CollectorListSessionsRequest
{
    public long UserId { get; set; }
    public string? CurrentTokenFamilyId { get; set; }

    public CollectorListSessionsDto FromRequest() => new()
    {
        UserId = UserId,
        CurrentTokenFamilyId = CurrentTokenFamilyId
    };
}

public class CollectorListSessionsRequestValidator : AbstractValidator<CollectorListSessionsRequest>
{
    public CollectorListSessionsRequestValidator()
    {

    }
}
