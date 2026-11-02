using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Settings.OperatingHours;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.OperatingHours;

public class UpdateOperatingHoursRequest
{
    public const string Permission = "edit_operating_hour";

    [FromRoute]
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }

    public UpdateOperatingHoursDto FromRequest() => new()
    {
        Id = Id,
        NameEn = NameEn,
        NameAr = NameAr
    };
}

public class UpdateOperatingHoursRequestValidator : AbstractValidator<UpdateOperatingHoursRequest>
{
    public UpdateOperatingHoursRequestValidator(IOperatingHoursRepository operatingHoursRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Operating hours ID");
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async (nameEn, excludeId) =>
            {
                var existing = await operatingHoursRepository.GetByNameAsync(nameEn);
                return existing == null || existing.Id == excludeId;
            }, x => x.Id, _ => "operating_hours.name_already_exists");

        RuleFor(x => x.NameAr)
            .IsOptionalArabicName()
            .When(x => !string.IsNullOrWhiteSpace(x.NameAr));
    }
}
