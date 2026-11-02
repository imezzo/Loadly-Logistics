using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Tags;

public class GetTagRequest
{
    public const string Permission = "view_tag";
    public long Id { get; set; }
    public GetTagDto FromRequest() => new() { Id = Id };
}

public class GetTagRequestValidator : AbstractValidator<GetTagRequest>
{
    public GetTagRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Tag ID");
    }
}
