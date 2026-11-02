using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.IndustryTypes;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.IndustryTypes;

public class CreateIndustryTypeRequest
{
    public const string Permission = "create_industry_type";

    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }

    public CreateIndustryTypeDto FromRequest() => new()
    {
        NameEn = NameEn,
        NameAr = NameAr
    };
}

public class CreateIndustryTypeRequestValidator : AbstractValidator<CreateIndustryTypeRequest>
{
    public CreateIndustryTypeRequestValidator(IIndustryTypeRepository industryTypeRepository)
    {
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async nameEn => await industryTypeRepository.GetByNameAsync(nameEn) == null, _ => "industry_types.name_already_exists");

        RuleFor(x => x.NameAr)
            .IsOptionalArabicName()
            .When(x => !string.IsNullOrWhiteSpace(x.NameAr));
    }
}
