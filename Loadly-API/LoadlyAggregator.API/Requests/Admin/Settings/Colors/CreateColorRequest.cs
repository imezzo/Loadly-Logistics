using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Colors;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Colors;

public class CreateColorRequest
{
    public const string Permission = "create_color";

    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string ColorHash { get; set; } = string.Empty;

    public CreateColorDto FromRequest() => new()
    {
        NameAr = NameAr,
        NameEn = NameEn,
        ColorHash = ColorHash
    };
}

public class CreateColorRequestValidator : AbstractValidator<CreateColorRequest>
{
    public CreateColorRequestValidator(IColorRepository colorRepository)
    {
        RuleFor(x => x.NameEn).IsRequiredEnglishName();
        RuleFor(x => x.NameAr).IsRequiredArabicName();
        RuleFor(x => x.ColorHash)
            .IsRequiredString(7, "Color hash")
            .Matches(@"^#[0-9A-Fa-f]{6}$").WithMessage("validation.color_hash_hex_invalid")
            .IsUnique(async colorHash => await colorRepository.GetByColorHashAsync(colorHash) == null, _ => "colors.color_hash_already_exists");
    }
}
