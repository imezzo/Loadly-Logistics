using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings;

public class CreateVehicleBrandModelRequest
{
    public const string Permission = "create_vehicle_brand_model";

    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public long VehicleBrandId { get; set; }

    public CreateVehicleBrandModelDto FromRequest() => new()
    {
        NameAr = NameAr,
        NameEn = NameEn,
        VehicleBrandId = VehicleBrandId
    };
}

public class CreateVehicleBrandModelRequestValidator : AbstractValidator<CreateVehicleBrandModelRequest>
{
    public CreateVehicleBrandModelRequestValidator(IVehicleBrandModelRepository vehicleBrandModelRepository)
    {
        RuleFor(x => x.NameAr).IsRequiredArabicName();
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .MustAsync(async (request, nameEn, cancellationToken) =>
            {
                var existing = await vehicleBrandModelRepository.GetByNameEnAndBrandIdAsync(nameEn!, request.VehicleBrandId);
                return existing == null;
            })
            .WithMessage("vehicle_brand_models.name_already_exists");
        RuleFor(x => x.VehicleBrandId).IsRequiredId("Vehicle brand ID");
    }
}
