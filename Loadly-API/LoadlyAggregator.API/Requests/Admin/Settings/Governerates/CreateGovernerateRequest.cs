using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Governerates;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Governerates;

public class CreateGovernerateRequest
{
    public const string Permission = "create_governerate";

    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }

    public CreateGovernerateDto FromRequest() => new()
    {
        NameEn = NameEn,
        NameAr = NameAr
    };
}

public class CreateGovernerateRequestValidator : AbstractValidator<CreateGovernerateRequest>
{
    public CreateGovernerateRequestValidator(IGovernerateRepository governerateRepository)
    {
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async nameEn => await governerateRepository.GetByNameAsync(nameEn) == null, _ => "governerates.name_already_exists");
        RuleFor(x => x.NameAr)
            .IsOptionalArabicName()
            .When(x => !string.IsNullOrWhiteSpace(x.NameAr));
    }
}
