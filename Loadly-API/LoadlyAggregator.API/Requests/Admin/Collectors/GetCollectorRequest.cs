using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Collectors;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Collectors;

namespace LoadlyAggregator.API.Requests.Admin.Collectors;

public class GetCollectorRequest
{
    public long Id { get; set; }

    public GetCollectorDto FromRequest() => new() { Id = Id };
}

public class GetCollectorRequestValidator : AbstractValidator<GetCollectorRequest>
{
    public GetCollectorRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Admin ID");
    }
}
