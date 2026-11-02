using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.VehicleBrands;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.VehicleBrands;

public class CreateVehicleBrandRequest
{
    public const string Permission = "create_vehicle_brand";

    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Logo { get; set; }

    public CreateVehicleBrandDto FromRequest() => new()
    {
        NameAr = NameAr,
        NameEn = NameEn,
        Logo = Logo
    };
}

public class CreateVehicleBrandRequestValidator : AbstractValidator<CreateVehicleBrandRequest>
{
    public CreateVehicleBrandRequestValidator(IVehicleBrandRepository vehicleBrandRepository)
    {
        RuleFor(x => x.NameAr).IsRequiredArabicName();
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async nameEn => await vehicleBrandRepository.GetByNameEnAsync(nameEn) == null, _ => "vehicle_brands.name_already_exists");
        RuleFor(x => x.Logo)
            .IsOptionalStringWithMaxLength(500, "Logo path")
            .When(x => !string.IsNullOrWhiteSpace(x.Logo));
    }
}
