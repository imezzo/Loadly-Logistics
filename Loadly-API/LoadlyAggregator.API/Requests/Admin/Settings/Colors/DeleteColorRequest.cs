using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Colors;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Colors;

public class DeleteColorRequest
{
    public const string Permission = "delete_color";
    public long Id { get; set; }
    public DeleteColorDto FromRequest() => new() { Id = Id };
}

public class DeleteColorRequestValidator : AbstractValidator<DeleteColorRequest>
{
    public DeleteColorRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Color ID");
    }
}
