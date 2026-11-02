using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Settings.Governerates;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Governerates;

public class UpdateGovernerateRequest
{
    public const string Permission = "edit_governerate";

    [FromRoute]
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }

    public UpdateGovernerateDto FromRequest() => new()
    {
        Id = Id,
        NameEn = NameEn,
        NameAr = NameAr
    };
}

public class UpdateGovernerateRequestValidator : AbstractValidator<UpdateGovernerateRequest>
{
    public UpdateGovernerateRequestValidator(IGovernerateRepository governerateRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Governerate ID");
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async (nameEn, excludeId) =>
            {
                var existing = await governerateRepository.GetByNameAsync(nameEn);
                return existing == null || existing.Id == excludeId;
            }, x => x.Id, _ => "governerates.name_already_exists");
        RuleFor(x => x.NameAr)
            .IsOptionalArabicName()
            .When(x => !string.IsNullOrWhiteSpace(x.NameAr));
    }
}
