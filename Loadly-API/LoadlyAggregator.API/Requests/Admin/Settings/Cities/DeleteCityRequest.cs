using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Cities;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Cities;

public class DeleteCityRequest
{
    public const string Permission = "delete_city";
    public long Id { get; set; }
    public DeleteCityDto FromRequest() => new() { Id = Id };
}

public class DeleteCityRequestValidator : AbstractValidator<DeleteCityRequest>
{
    public DeleteCityRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("City ID");
    }
}
