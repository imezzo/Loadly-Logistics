using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Containers;

public class UpdateContainerRequest
{
    public const string Permission = "edit_container";

    [FromRoute]
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public ContainerStatus Status { get; set; }

    public UpdateContainerDto FromRequest() => new()
    {
        Id = Id,
        NameAr = NameAr,
        NameEn = NameEn,
        Status = Status
    };
}

public class UpdateContainerRequestValidator : AbstractValidator<UpdateContainerRequest>
{
    public UpdateContainerRequestValidator(IContainerRepository containerRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Container ID");
        RuleFor(x => x.NameAr)
            .IsRequiredArabicName()
            .IsUnique(async (nameAr, excludeId) =>
            {
                Container? existing = await containerRepository.GetByNameArAsync(nameAr);
                return existing == null || existing.Id == excludeId;
            }, x => x.Id, _ => "containers.name_ar_already_exists");
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async (nameEn, excludeId) =>
            {
                Container? existing = await containerRepository.GetByNameEnAsync(nameEn);
                return existing == null || existing.Id == excludeId;
            }, x => x.Id, _ => "containers.name_en_already_exists");
        RuleFor(x => x.Status)
            .IsValidEnum<UpdateContainerRequest, ContainerStatus>("Status");
    }
}
