using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Sectors;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Sectors;

public class DeleteSectorRequest
{
    public const string Permission = "delete_sector";
    public long Id { get; set; }
    public DeleteSectorDto FromRequest() => new() { Id = Id };
}

public class DeleteSectorRequestValidator : AbstractValidator<DeleteSectorRequest>
{
    public DeleteSectorRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Sector ID");
    }
}
