using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Discounts;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Discounts;

public class CreateDiscountRequest
{
    public const string Permission = "create_discount";

    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public DiscountType Type { get; set; }
    public decimal Value { get; set; }
    public DiscountStatus Status { get; set; } = DiscountStatus.ACTIVE;

    public CreateDiscountDto FromRequest() => new()
    {
        NameAr = NameAr,
        NameEn = NameEn,
        Type = Type,
        Value = Value,
        Status = Status
    };
}

public class CreateDiscountRequestValidator : AbstractValidator<CreateDiscountRequest>
{
    public CreateDiscountRequestValidator(IDiscountRepository discountRepository)
    {
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async nameEn => await discountRepository.GetByNameAsync(nameEn) == null, _ => "discounts.name_already_exists");
        RuleFor(x => x.NameAr).IsRequiredArabicName();
        RuleFor(x => x.Type)
            .IsValidEnum<CreateDiscountRequest, Domain.Common.Enums.DiscountType>("Type");
        RuleFor(x => x.Value)
            .IsGreaterThanOrEqualToZero("Value");
        RuleFor(x => x.Status)
            .IsValidEnum<CreateDiscountRequest, Domain.Common.Enums.DiscountStatus>("Status");
    }
}
