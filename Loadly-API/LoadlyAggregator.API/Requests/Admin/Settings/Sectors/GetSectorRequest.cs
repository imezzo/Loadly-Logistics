using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Sectors;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Sectors;

public class GetSectorRequest
{
    public const string Permission = "view_sector";
    public long Id { get; set; }
    public GetSectorDto FromRequest() => new() { Id = Id };
}

public class GetSectorRequestValidator : AbstractValidator<GetSectorRequest>
{
    public GetSectorRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Sector ID");
    }
}
