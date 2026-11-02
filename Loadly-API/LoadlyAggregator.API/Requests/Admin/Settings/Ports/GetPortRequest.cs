using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Ports;

public class GetPortRequest
{
    public const string Permission = "view_port";

    public long Id { get; set; }

    public GetPortDto FromRequest() => new() { Id = Id };
}

public class GetPortRequestValidator : AbstractValidator<GetPortRequest>
{
    public GetPortRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Port ID");
    }
}
