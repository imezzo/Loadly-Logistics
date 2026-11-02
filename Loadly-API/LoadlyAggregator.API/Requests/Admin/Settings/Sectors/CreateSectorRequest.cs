using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Sectors;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Sectors;

public class CreateSectorRequest
{
    public const string Permission = "create_sector";

    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }

    public CreateSectorDto FromRequest() => new()
    {
        NameEn = NameEn,
        NameAr = NameAr
    };
}

public class CreateSectorRequestValidator : AbstractValidator<CreateSectorRequest>
{
    public CreateSectorRequestValidator(ISectorRepository sectorRepository)
    {
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async nameEn => await sectorRepository.GetByNameAsync(nameEn) == null, _ => "sectors.name_already_exists");
        RuleFor(x => x.NameAr)
            .IsOptionalArabicName()
            .When(x => !string.IsNullOrWhiteSpace(x.NameAr));
    }
}
