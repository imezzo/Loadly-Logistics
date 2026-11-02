using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Tags;

public class UpdateTagRequest
{
    public const string Permission = "edit_tag";

    [FromRoute]
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public bool IsPricing { get; set; }

    public UpdateTagDto FromRequest() => new()
    {
        Id = Id,
        NameAr = NameAr,
        NameEn = NameEn,
        IsPricing = IsPricing
    };
}

public class UpdateTagRequestValidator : AbstractValidator<UpdateTagRequest>
{
    public UpdateTagRequestValidator(ITagRepository tagRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Tag ID");
        RuleFor(x => x.NameAr).IsRequiredArabicName();
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async (nameEn, excludeId) =>
            {
                var existing = await tagRepository.GetByNameEnAsync(nameEn);
                return existing == null || existing.Id == excludeId;
            }, x => x.Id, _ => "tags.name_already_exists");
    }
}
