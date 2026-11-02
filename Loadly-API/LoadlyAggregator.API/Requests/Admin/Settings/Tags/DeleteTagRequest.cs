using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Tags;

public class DeleteTagRequest
{
    public const string Permission = "delete_tag";
    public long Id { get; set; }
    public DeleteTagDto FromRequest() => new() { Id = Id };
}

public class DeleteTagRequestValidator : AbstractValidator<DeleteTagRequest>
{
    public DeleteTagRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Tag ID");
    }
}
