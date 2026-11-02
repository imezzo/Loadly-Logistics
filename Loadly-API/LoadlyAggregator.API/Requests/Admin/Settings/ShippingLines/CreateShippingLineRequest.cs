using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.ShippingLines;

public class CreateShippingLineRequest
{
    public const string Permission = "create_shipping_line";

    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public ShippingLineStatus Status { get; set; } = ShippingLineStatus.ACTIVE;

    public CreateShippingLineDto FromRequest() => new()
    {
        NameAr = NameAr,
        NameEn = NameEn,
        Status = Status
    };
}

public class CreateShippingLineRequestValidator : AbstractValidator<CreateShippingLineRequest>
{
    public CreateShippingLineRequestValidator(IShippingLineRepository shippingLineRepository)
    {
        RuleFor(x => x.NameAr)
            .IsRequiredArabicName()
            .IsUnique(async nameAr => await shippingLineRepository.GetByNameArAsync(nameAr) == null,
                _ => "shipping_lines.name_ar_already_exists");
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async nameEn => await shippingLineRepository.GetByNameEnAsync(nameEn) == null,
                _ => "shipping_lines.name_en_already_exists");
        RuleFor(x => x.Status)
            .IsValidEnum<CreateShippingLineRequest, ShippingLineStatus>("Status");
    }
}
