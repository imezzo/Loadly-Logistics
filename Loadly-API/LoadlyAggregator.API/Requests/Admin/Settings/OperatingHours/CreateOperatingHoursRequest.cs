using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.OperatingHours;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.OperatingHours;

public class CreateOperatingHoursRequest
{
    public const string Permission = "create_operating_hour";

    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }

    public CreateOperatingHoursDto FromRequest() => new()
    {
        NameEn = NameEn,
        NameAr = NameAr
    };
}

public class CreateOperatingHoursRequestValidator : AbstractValidator<CreateOperatingHoursRequest>
{
    public CreateOperatingHoursRequestValidator(IOperatingHoursRepository operatingHoursRepository)
    {
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async nameEn => await operatingHoursRepository.GetByNameAsync(nameEn) == null, _ => "operating_hours.name_already_exists");

        RuleFor(x => x.NameAr)
            .IsOptionalArabicName()
            .When(x => !string.IsNullOrWhiteSpace(x.NameAr));
    }
}
