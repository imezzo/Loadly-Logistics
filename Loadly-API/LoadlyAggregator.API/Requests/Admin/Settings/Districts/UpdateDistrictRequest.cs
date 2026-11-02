using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Settings.Districts;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Districts;

public class UpdateDistrictRequest
{
    public const string Permission = "edit_district";

    [FromRoute]
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public long CityId { get; set; }

    public UpdateDistrictDto FromRequest() => new()
    {
        Id = Id,
        NameEn = NameEn,
        NameAr = NameAr,
        CityId = CityId
    };
}

public class UpdateDistrictRequestValidator : AbstractValidator<UpdateDistrictRequest>
{
    public UpdateDistrictRequestValidator(IDistrictRepository districtRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("District ID");
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .MustAsync(async (request, nameEn, cancellationToken) =>
            {
                var existing = await districtRepository.GetByNameAndCityAsync(nameEn!, request.CityId);
                return existing == null || existing.Id == request.Id;
            })
            .WithMessage("districts.name_already_exists");
        RuleFor(x => x.NameAr)
            .IsOptionalArabicName()
            .When(x => !string.IsNullOrWhiteSpace(x.NameAr));
        RuleFor(x => x.CityId).IsRequiredId("City ID");
    }
}
