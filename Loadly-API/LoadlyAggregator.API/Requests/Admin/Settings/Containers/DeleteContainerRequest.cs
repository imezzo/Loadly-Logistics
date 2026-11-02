using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Containers;

public class DeleteContainerRequest
{
    public const string Permission = "delete_container";

    public long Id { get; set; }

    public DeleteContainerDto FromRequest() => new() { Id = Id };
}

public class DeleteContainerRequestValidator : AbstractValidator<DeleteContainerRequest>
{
    public DeleteContainerRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Container ID");
    }
}
