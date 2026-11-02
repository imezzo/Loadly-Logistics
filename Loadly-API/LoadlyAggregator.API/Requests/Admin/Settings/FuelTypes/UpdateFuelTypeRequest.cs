using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Settings.FuelTypes;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.FuelTypes;

public class UpdateFuelTypeRequest
{
    public const string Permission = "edit_fuel_type";

    [FromRoute]
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }

    public UpdateFuelTypeDto FromRequest() => new()
    {
        Id = Id,
        NameAr = NameAr,
        NameEn = NameEn,
        UnitPrice = UnitPrice
    };
}

public class UpdateFuelTypeRequestValidator : AbstractValidator<UpdateFuelTypeRequest>
{
    public UpdateFuelTypeRequestValidator(IFuelTypeRepository fuelTypeRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Fuel type ID");
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async (nameEn, excludeId) =>
            {
                var existing = await fuelTypeRepository.GetByNameEnAsync(nameEn);
                return existing == null || existing.Id == excludeId;
            }, x => x.Id, value => $"fuel_types.name_en_already_exists|{value}");
        RuleFor(x => x.NameAr)
            .IsRequiredArabicName()
            .IsUnique(async (nameAr, excludeId) =>
            {
                var existing = await fuelTypeRepository.GetByNameArAsync(nameAr);
                return existing == null || existing.Id == excludeId;
            }, x => x.Id, value => $"fuel_types.name_ar_already_exists|{value}");
    }
}
