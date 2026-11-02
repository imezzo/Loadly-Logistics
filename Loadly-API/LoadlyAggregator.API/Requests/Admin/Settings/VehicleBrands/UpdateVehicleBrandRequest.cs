using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Settings.VehicleBrands;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.VehicleBrands;

public class UpdateVehicleBrandRequest
{
    public const string Permission = "edit_vehicle_brand";

    [FromRoute]
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Logo { get; set; }

    public UpdateVehicleBrandDto FromRequest() => new()
    {
        Id = Id,
        NameAr = NameAr,
        NameEn = NameEn,
        Logo = Logo
    };
}

public class UpdateVehicleBrandRequestValidator : AbstractValidator<UpdateVehicleBrandRequest>
{
    public UpdateVehicleBrandRequestValidator(IVehicleBrandRepository vehicleBrandRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Vehicle brand ID");
        RuleFor(x => x.NameAr).IsRequiredArabicName();
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async (nameEn, excludeId) =>
            {
                var existing = await vehicleBrandRepository.GetByNameEnAsync(nameEn);
                return existing == null || existing.Id == excludeId;
            }, x => x.Id, _ => "vehicle_brands.name_already_exists");
        RuleFor(x => x.Logo)
            .IsOptionalStringWithMaxLength(500, "Logo path")
            .When(x => !string.IsNullOrWhiteSpace(x.Logo));
    }
}
