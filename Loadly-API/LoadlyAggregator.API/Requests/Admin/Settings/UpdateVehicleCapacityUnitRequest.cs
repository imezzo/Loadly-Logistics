using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Settings;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings;

public class UpdateVehicleCapacityUnitRequest
{
    public const string Permission = "edit_vehicle_capacity_unit";

    [FromRoute]
    public long Id { get; set; }
    public string CapacityTypeEn { get; set; } = string.Empty;
    public string CapacityTypeAr { get; set; } = string.Empty;

    public UpdateVehicleCapacityUnitDto FromRequest() => new()
    {
        Id = Id,
        CapacityTypeEn = CapacityTypeEn,
        CapacityTypeAr = CapacityTypeAr
    };
}

public class UpdateVehicleCapacityUnitRequestValidator : AbstractValidator<UpdateVehicleCapacityUnitRequest>
{
    public UpdateVehicleCapacityUnitRequestValidator(IVehicleCapacityUnitRepository vehicleCapacityUnitRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Vehicle capacity unit ID");
        RuleFor(x => x.CapacityTypeEn)
            .IsRequiredEnglishName()
            .IsUnique(async (capacityTypeEn, excludeId) =>
            {
                var existing = await vehicleCapacityUnitRepository.GetByCapacityTypeEnAsync(capacityTypeEn);
                return existing == null || existing.Id == excludeId;
            }, x => x.Id, _ => "vehicle_capacity_units.name_already_exists");
        RuleFor(x => x.CapacityTypeAr).IsRequiredArabicName();
    }
}
