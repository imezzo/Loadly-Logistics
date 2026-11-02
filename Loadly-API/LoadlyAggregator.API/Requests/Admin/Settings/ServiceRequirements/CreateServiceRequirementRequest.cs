using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.ServiceRequirements;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.ServiceRequirements;

public class CreateServiceRequirementRequest
{
    public const string Permission = "create_service_requirement";

    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }

    public CreateServiceRequirementDto FromRequest() => new()
    {
        NameEn = NameEn,
        NameAr = NameAr
    };
}

public class CreateServiceRequirementRequestValidator : AbstractValidator<CreateServiceRequirementRequest>
{
    public CreateServiceRequirementRequestValidator(IServiceRequirementRepository serviceRequirementRepository)
    {
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async nameEn => await serviceRequirementRepository.GetByNameAsync(nameEn) == null, _ => "service_requirements.name_already_exists");

        RuleFor(x => x.NameAr)
            .IsOptionalArabicName()
            .When(x => !string.IsNullOrWhiteSpace(x.NameAr));
    }
}
