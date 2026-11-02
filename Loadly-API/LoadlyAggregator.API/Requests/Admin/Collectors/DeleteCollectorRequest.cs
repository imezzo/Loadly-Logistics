using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Collectors;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Collectors;

namespace LoadlyAggregator.API.Requests.Admin.Collectors;

public class DeleteCollectorRequest
{
    public long Id { get; set; }

    public DeleteCollectorDto FromRequest() => new() { Id = Id };
}

public class DeleteCollectorRequestValidator : AbstractValidator<DeleteCollectorRequest>
{
    public DeleteCollectorRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Admin ID");
    }
}
