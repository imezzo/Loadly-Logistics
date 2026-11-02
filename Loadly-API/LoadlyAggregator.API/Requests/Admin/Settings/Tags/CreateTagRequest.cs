using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Tags;

public class CreateTagRequest
{
    public const string Permission = "create_tag";

    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public bool IsPricing { get; set; } = false;

    public CreateTagDto FromRequest() => new()
    {
        NameAr = NameAr,
        NameEn = NameEn,
        IsPricing = IsPricing
    };
}

public class CreateTagRequestValidator : AbstractValidator<CreateTagRequest>
{
    public CreateTagRequestValidator(ITagRepository tagRepository)
    {
        RuleFor(x => x.NameAr).IsRequiredArabicName();
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async nameEn => await tagRepository.GetByNameEnAsync(nameEn) == null, _ => "tags.name_already_exists");
    }
}
