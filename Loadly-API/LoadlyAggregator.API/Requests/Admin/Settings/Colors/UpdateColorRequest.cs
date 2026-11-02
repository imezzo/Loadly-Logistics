using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Settings.Colors;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Colors;

public class UpdateColorRequest
{
    public const string Permission = "edit_color";

    [FromRoute]
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string ColorHash { get; set; } = string.Empty;

    public UpdateColorDto FromRequest() => new()
    {
        Id = Id,
        NameAr = NameAr,
        NameEn = NameEn,
        ColorHash = ColorHash
    };
}

public class UpdateColorRequestValidator : AbstractValidator<UpdateColorRequest>
{
    public UpdateColorRequestValidator(IColorRepository colorRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Color ID");
        RuleFor(x => x.NameEn).IsRequiredEnglishName();
        RuleFor(x => x.NameAr).IsRequiredArabicName();
        RuleFor(x => x.ColorHash)
            .IsRequiredString(7, "Color hash")
            .Matches(@"^#[0-9A-Fa-f]{6}$").WithMessage("validation.color_hash_hex_invalid")
            .IsUnique(async (colorHash, excludeId) =>
            {
                var existing = await colorRepository.GetByColorHashAsync(colorHash);
                return existing == null || existing.Id == excludeId;
            }, x => x.Id, _ => "colors.color_hash_already_exists");
    }
}
