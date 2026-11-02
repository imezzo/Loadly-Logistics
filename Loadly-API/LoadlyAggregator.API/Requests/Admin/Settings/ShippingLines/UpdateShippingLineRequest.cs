using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.ShippingLines;

public class UpdateShippingLineRequest
{
    public const string Permission = "edit_shipping_line";

    [FromRoute]
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public ShippingLineStatus Status { get; set; }

    public UpdateShippingLineDto FromRequest() => new()
    {
        Id = Id,
        NameAr = NameAr,
        NameEn = NameEn,
        Status = Status
    };
}

public class UpdateShippingLineRequestValidator : AbstractValidator<UpdateShippingLineRequest>
{
    public UpdateShippingLineRequestValidator(IShippingLineRepository shippingLineRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Shipping line ID");
        RuleFor(x => x.NameAr)
            .IsRequiredArabicName()
            .IsUnique(async (nameAr, excludeId) =>
            {
                ShippingLine? existing = await shippingLineRepository.GetByNameArAsync(nameAr);
                return existing == null || existing.Id == excludeId;
            }, x => x.Id, _ => "shipping_lines.name_ar_already_exists");
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async (nameEn, excludeId) =>
            {
                ShippingLine? existing = await shippingLineRepository.GetByNameEnAsync(nameEn);
                return existing == null || existing.Id == excludeId;
            }, x => x.Id, _ => "shipping_lines.name_en_already_exists");
        RuleFor(x => x.Status)
            .IsValidEnum<UpdateShippingLineRequest, ShippingLineStatus>("Status");
    }
}
