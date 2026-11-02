using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Settings.VehicleTypes;

public class CreateVehicleTypeRequest
{
    public const string Permission = "create_vehicle_type";

    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public VehicleTypeCategory Category { get; set; }
    public VehicleUnitType? VehicleUnitType { get; set; }
    public VehicleTypeStatus Status { get; set; } = VehicleTypeStatus.ACTIVE;
    public List<long> TagIds { get; set; } = [];

    public CreateVehicleTypeDto FromRequest() => new()
    {
        NameAr = NameAr,
        NameEn = NameEn,
        Category = Category,
        Status = Status,
        VehicleUnitType = VehicleUnitType,
        TagIds = TagIds.Distinct().ToList(),
    };
}

public class CreateVehicleTypeRequestValidator : AbstractValidator<CreateVehicleTypeRequest>
{
    public CreateVehicleTypeRequestValidator()
    {
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName();
        RuleFor(x => x.NameAr).IsRequiredArabicName();
        RuleFor(x => x.Category)
            .IsValidEnum<CreateVehicleTypeRequest, VehicleTypeCategory>("Category");
        RuleFor(x => x.Status)
            .IsValidEnum<CreateVehicleTypeRequest, VehicleTypeStatus>("Status");
        RuleFor(x => x.VehicleUnitType)
            .IsValidEnum<CreateVehicleTypeRequest, VehicleUnitType>("VehicleUnitType")
            .When(x => x.VehicleUnitType.HasValue);

        RuleFor(x => x.TagIds)
            .IsRequiredNonEmptyCollection("TagIds");
    }
}
