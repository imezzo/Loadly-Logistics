using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Cities;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Cities;

public class CreateCityRequest
{
    public const string Permission = "create_city";

    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public long GovernerateId { get; set; }

    public CreateCityDto FromRequest() => new()
    {
        NameEn = NameEn,
        NameAr = NameAr,
        GovernerateId = GovernerateId
    };
}

public class CreateCityRequestValidator : AbstractValidator<CreateCityRequest>
{
    public CreateCityRequestValidator(ICityRepository cityRepository)
    {
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .MustAsync(async (request, nameEn, cancellationToken) =>
            {
                var existing = await cityRepository.GetByNameAndGovernerateAsync(nameEn!, request.GovernerateId);
                return existing == null;
            })
            .WithMessage("cities.name_already_exists");
        RuleFor(x => x.NameAr)
            .IsOptionalArabicName()
            .When(x => !string.IsNullOrWhiteSpace(x.NameAr));
        RuleFor(x => x.GovernerateId).IsRequiredId("Governerate ID");
    }
}
