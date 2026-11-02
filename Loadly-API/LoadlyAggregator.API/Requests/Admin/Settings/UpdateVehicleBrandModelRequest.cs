using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Settings;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings;

public class UpdateVehicleBrandModelRequest
{
    public const string Permission = "edit_vehicle_brand_model";

    [FromRoute]
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public long VehicleBrandId { get; set; }

    public UpdateVehicleBrandModelDto FromRequest() => new()
    {
        Id = Id,
        NameAr = NameAr,
        NameEn = NameEn,
        VehicleBrandId = VehicleBrandId
    };
}

public class UpdateVehicleBrandModelRequestValidator : AbstractValidator<UpdateVehicleBrandModelRequest>
{
    public UpdateVehicleBrandModelRequestValidator(IVehicleBrandModelRepository vehicleBrandModelRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Vehicle brand model ID");
        RuleFor(x => x.NameAr).IsRequiredArabicName();
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .MustAsync(async (request, nameEn, cancellationToken) =>
            {
                var existing = await vehicleBrandModelRepository.GetByNameEnAndBrandIdAsync(nameEn!, request.VehicleBrandId);
                return existing == null || existing.Id == request.Id;
            })
            .WithMessage("vehicle_brand_models.name_already_exists");
        RuleFor(x => x.VehicleBrandId).IsRequiredId("Vehicle brand ID");
    }
}
