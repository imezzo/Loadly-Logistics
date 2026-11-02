using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.FuelTypes;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.FuelTypes;

public class CreateFuelTypeRequest
{
    public const string Permission = "create_fuel_type";

    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }

    public CreateFuelTypeDto FromRequest() => new()
    {
        NameAr = NameAr,
        NameEn = NameEn,
        UnitPrice = UnitPrice
    };
}

public class CreateFuelTypeRequestValidator : AbstractValidator<CreateFuelTypeRequest>
{
    public CreateFuelTypeRequestValidator(IFuelTypeRepository fuelTypeRepository)
    {
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async nameEn => await fuelTypeRepository.GetByNameEnAsync(nameEn) == null, value => $"fuel_types.name_en_already_exists|{value}");
        RuleFor(x => x.NameAr)
            .IsRequiredArabicName()
            .IsUnique(async nameAr => await fuelTypeRepository.GetByNameArAsync(nameAr) == null, value => $"fuel_types.name_ar_already_exists|{value}");
    }
}
