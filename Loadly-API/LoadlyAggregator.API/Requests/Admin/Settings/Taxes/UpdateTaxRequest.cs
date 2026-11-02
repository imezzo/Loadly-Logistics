using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Settings.Taxes;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Taxes;

public class UpdateTaxRequest
{
    public const string Permission = "edit_tax";

    [FromRoute]
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public TaxType Type { get; set; }
    public decimal Value { get; set; }
    public TaxStatus Status { get; set; }

    public UpdateTaxDto FromRequest() => new()
    {
        Id = Id,
        NameAr = NameAr,
        NameEn = NameEn,
        Type = Type,
        Value = Value,
        Status = Status
    };
}

public class UpdateTaxRequestValidator : AbstractValidator<UpdateTaxRequest>
{
    public UpdateTaxRequestValidator(ITaxRepository taxRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Tax ID");
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async (nameEn, excludeId) =>
            {
                var existing = await taxRepository.GetByNameAsync(nameEn);
                return existing == null || existing.Id == excludeId;
            }, x => x.Id, _ => "taxes.name_already_exists");
        RuleFor(x => x.NameAr).IsRequiredArabicName();
        RuleFor(x => x.Type)
            .IsValidEnum<UpdateTaxRequest, Domain.Common.Enums.TaxType>("Type");
        RuleFor(x => x.Value)
            .IsGreaterThanOrEqualToZero("Value");
        RuleFor(x => x.Status)
            .IsValidEnum<UpdateTaxRequest, Domain.Common.Enums.TaxStatus>("Status");
    }
}
