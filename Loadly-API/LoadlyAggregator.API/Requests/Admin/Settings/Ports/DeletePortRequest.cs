using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Ports;

public class DeletePortRequest
{
    public const string Permission = "delete_port";

    public long Id { get; set; }

    public DeletePortDto FromRequest() => new() { Id = Id };
}

public class DeletePortRequestValidator : AbstractValidator<DeletePortRequest>
{
    public DeletePortRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Port ID");
    }
}
