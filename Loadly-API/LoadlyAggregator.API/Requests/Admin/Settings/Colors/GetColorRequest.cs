using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Colors;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Colors;

public class GetColorRequest
{
    public const string Permission = "view_color";
    public long Id { get; set; }
    public GetColorDto FromRequest() => new() { Id = Id };
}

public class GetColorRequestValidator : AbstractValidator<GetColorRequest>
{
    public GetColorRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Color ID");
    }
}
