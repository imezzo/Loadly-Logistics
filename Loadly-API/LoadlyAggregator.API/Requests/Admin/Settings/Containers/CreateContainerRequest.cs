using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Containers;

public class CreateContainerRequest
{
    public const string Permission = "create_container";

    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public ContainerStatus Status { get; set; } = ContainerStatus.ACTIVE;

    public CreateContainerDto FromRequest() => new()
    {
        NameAr = NameAr,
        NameEn = NameEn,
        Status = Status
    };
}

public class CreateContainerRequestValidator : AbstractValidator<CreateContainerRequest>
{
    public CreateContainerRequestValidator(IContainerRepository containerRepository)
    {
        RuleFor(x => x.NameAr)
            .IsRequiredArabicName()
            .IsUnique(async nameAr => await containerRepository.GetByNameArAsync(nameAr) == null,
                _ => "containers.name_ar_already_exists");
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async nameEn => await containerRepository.GetByNameEnAsync(nameEn) == null,
                _ => "containers.name_en_already_exists");
        RuleFor(x => x.Status)
            .IsValidEnum<CreateContainerRequest, ContainerStatus>("Status");
    }
}
