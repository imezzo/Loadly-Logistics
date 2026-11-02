using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Containers;

public class GetContainerRequest
{
    public const string Permission = "view_container";

    public long Id { get; set; }

    public GetContainerDto FromRequest() => new() { Id = Id };
}

public class GetContainerRequestValidator : AbstractValidator<GetContainerRequest>
{
    public GetContainerRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Container ID");
    }
}
