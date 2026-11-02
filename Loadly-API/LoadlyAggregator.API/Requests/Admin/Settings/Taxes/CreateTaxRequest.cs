using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Taxes;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Taxes;

public class CreateTaxRequest
{
    public const string Permission = "create_tax";

    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public TaxType Type { get; set; }
    public decimal Value { get; set; }
    public TaxStatus Status { get; set; } = TaxStatus.ACTIVE;

    public CreateTaxDto FromRequest() => new()
    {
        NameAr = NameAr,
        NameEn = NameEn,
        Type = Type,
        Value = Value,
        Status = Status
    };
}

public class CreateTaxRequestValidator : AbstractValidator<CreateTaxRequest>
{
    public CreateTaxRequestValidator(ITaxRepository taxRepository)
    {
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async nameEn => await taxRepository.GetByNameAsync(nameEn) == null, _ => "taxes.name_already_exists");
        RuleFor(x => x.NameAr).IsRequiredArabicName();
        RuleFor(x => x.Type)
            .IsValidEnum<CreateTaxRequest, Domain.Common.Enums.TaxType>("Type");
        RuleFor(x => x.Value)
            .IsGreaterThanOrEqualToZero("Value");
        RuleFor(x => x.Status)
            .IsValidEnum<CreateTaxRequest, Domain.Common.Enums.TaxStatus>("Status");
    }
}
