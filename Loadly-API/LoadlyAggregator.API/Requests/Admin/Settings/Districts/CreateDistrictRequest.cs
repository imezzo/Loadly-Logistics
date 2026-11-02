using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Districts;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Districts;

public class CreateDistrictRequest
{
    public const string Permission = "create_district";

    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public long CityId { get; set; }

    public CreateDistrictDto FromRequest() => new()
    {
        NameEn = NameEn,
        NameAr = NameAr,
        CityId = CityId
    };
}

public class CreateDistrictRequestValidator : AbstractValidator<CreateDistrictRequest>
{
    public CreateDistrictRequestValidator(IDistrictRepository districtRepository)
    {
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .MustAsync(async (request, nameEn, cancellationToken) =>
            {
                var existing = await districtRepository.GetByNameAndCityAsync(nameEn!, request.CityId);
                return existing == null;
            })
            .WithMessage("districts.name_already_exists");
        RuleFor(x => x.NameAr)
            .IsOptionalArabicName()
            .When(x => !string.IsNullOrWhiteSpace(x.NameAr));
        RuleFor(x => x.CityId).IsRequiredId("City ID");
    }
}
