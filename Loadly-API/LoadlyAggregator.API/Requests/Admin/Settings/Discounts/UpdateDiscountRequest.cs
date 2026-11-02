using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Settings.Discounts;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Discounts;

public class UpdateDiscountRequest
{
    public const string Permission = "edit_discount";

    [FromRoute]
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public DiscountType Type { get; set; }
    public decimal Value { get; set; }
    public DiscountStatus Status { get; set; }

    public UpdateDiscountDto FromRequest() => new()
    {
        Id = Id,
        NameAr = NameAr,
        NameEn = NameEn,
        Type = Type,
        Value = Value,
        Status = Status
    };
}

public class UpdateDiscountRequestValidator : AbstractValidator<UpdateDiscountRequest>
{
    public UpdateDiscountRequestValidator(IDiscountRepository discountRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Discount ID");
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async (nameEn, excludeId) =>
            {
                var existing = await discountRepository.GetByNameAsync(nameEn);
                return existing == null || existing.Id == excludeId;
            }, x => x.Id, _ => "discounts.name_already_exists");
        RuleFor(x => x.NameAr).IsRequiredArabicName();
        RuleFor(x => x.Type)
            .IsValidEnum<UpdateDiscountRequest, Domain.Common.Enums.DiscountType>("Type");
        RuleFor(x => x.Value)
            .IsGreaterThanOrEqualToZero("Value");
        RuleFor(x => x.Status)
            .IsValidEnum<UpdateDiscountRequest, Domain.Common.Enums.DiscountStatus>("Status");
    }
}
