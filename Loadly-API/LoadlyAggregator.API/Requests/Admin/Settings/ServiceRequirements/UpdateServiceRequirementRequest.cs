using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Settings.ServiceRequirements;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.ServiceRequirements;

public class UpdateServiceRequirementRequest
{
    public const string Permission = "edit_service_requirement";

    [FromRoute]
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }

    public UpdateServiceRequirementDto FromRequest() => new()
    {
        Id = Id,
        NameEn = NameEn,
        NameAr = NameAr
    };
}

public class UpdateServiceRequirementRequestValidator : AbstractValidator<UpdateServiceRequirementRequest>
{
    public UpdateServiceRequirementRequestValidator(IServiceRequirementRepository serviceRequirementRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Service requirement ID");
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async (nameEn, excludeId) =>
            {
                var existing = await serviceRequirementRepository.GetByNameAsync(nameEn);
                return existing == null || existing.Id == excludeId;
            }, x => x.Id, _ => "service_requirements.name_already_exists");

        RuleFor(x => x.NameAr)
            .IsOptionalArabicName()
            .When(x => !string.IsNullOrWhiteSpace(x.NameAr));
    }
}
