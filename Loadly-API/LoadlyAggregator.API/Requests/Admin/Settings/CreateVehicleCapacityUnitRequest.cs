using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings;

public class CreateVehicleCapacityUnitRequest
{
    public const string Permission = "create_vehicle_capacity_unit";

    public string CapacityTypeEn { get; set; } = string.Empty;
    public string CapacityTypeAr { get; set; } = string.Empty;

    public CreateVehicleCapacityUnitDto FromRequest() => new()
    {
        CapacityTypeEn = CapacityTypeEn,
        CapacityTypeAr = CapacityTypeAr
    };
}

public class CreateVehicleCapacityUnitRequestValidator : AbstractValidator<CreateVehicleCapacityUnitRequest>
{
    public CreateVehicleCapacityUnitRequestValidator(IVehicleCapacityUnitRepository vehicleCapacityUnitRepository)
    {
        RuleFor(x => x.CapacityTypeEn)
            .IsRequiredEnglishName()
            .IsUnique(async capacityTypeEn => await vehicleCapacityUnitRepository.GetByCapacityTypeEnAsync(capacityTypeEn) == null, _ => "vehicle_capacity_units.name_already_exists");
        RuleFor(x => x.CapacityTypeAr)
            .IsRequiredArabicName();
    }
}
