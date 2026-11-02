using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Settings.Cities;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Cities;

public class UpdateCityRequest
{
    public const string Permission = "edit_city";

    [FromRoute]
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public long GovernerateId { get; set; }

    public UpdateCityDto FromRequest() => new()
    {
        Id = Id,
        NameEn = NameEn,
        NameAr = NameAr,
        GovernerateId = GovernerateId
    };
}

public class UpdateCityRequestValidator : AbstractValidator<UpdateCityRequest>
{
    public UpdateCityRequestValidator(ICityRepository cityRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("City ID");
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .MustAsync(async (request, nameEn, cancellationToken) =>
            {
                var existing = await cityRepository.GetByNameAndGovernerateAsync(nameEn!, request.GovernerateId);
                return existing == null || existing.Id == request.Id;
            })
            .WithMessage("cities.name_already_exists");
        RuleFor(x => x.NameAr)
            .IsOptionalArabicName()
            .When(x => !string.IsNullOrWhiteSpace(x.NameAr));
        RuleFor(x => x.GovernerateId).IsRequiredId("Governerate ID");
    }
}
