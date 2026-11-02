using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Settings.IndustryTypes;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.IndustryTypes;

public class UpdateIndustryTypeRequest
{
    public const string Permission = "edit_industry_type";

    [FromRoute]
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }

    public UpdateIndustryTypeDto FromRequest() => new()
    {
        Id = Id,
        NameEn = NameEn,
        NameAr = NameAr
    };
}

public class UpdateIndustryTypeRequestValidator : AbstractValidator<UpdateIndustryTypeRequest>
{
    public UpdateIndustryTypeRequestValidator(IIndustryTypeRepository industryTypeRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Industry type ID");
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async (nameEn, excludeId) =>
            {
                var existing = await industryTypeRepository.GetByNameAsync(nameEn);
                return existing == null || existing.Id == excludeId;
            }, x => x.Id, _ => "industry_types.name_already_exists");

        RuleFor(x => x.NameAr)
            .IsOptionalArabicName()
            .When(x => !string.IsNullOrWhiteSpace(x.NameAr));
    }
}
