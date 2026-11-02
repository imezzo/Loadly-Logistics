using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Districts;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Districts;

public class DeleteDistrictRequest
{
    public const string Permission = "delete_district";
    public long Id { get; set; }
    public DeleteDistrictDto FromRequest() => new() { Id = Id };
}

public class DeleteDistrictRequestValidator : AbstractValidator<DeleteDistrictRequest>
{
    public DeleteDistrictRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("District ID");
    }
}
