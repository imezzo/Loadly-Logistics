using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Settings.VehicleTypes;

public class UpdateVehicleTypeRequest
{
    public const string Permission = "edit_vehicle_type";

    [FromRoute]
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public VehicleTypeCategory Category { get; set; }
    public VehicleUnitType? VehicleUnitType { get; set; }
    public VehicleTypeStatus Status { get; set; }
    public List<long> TagIds { get; set; } = [];

    public UpdateVehicleTypeDto FromRequest() => new()
    {
        Id = Id,
        NameAr = NameAr,
        NameEn = NameEn,
        Category = Category,
        VehicleUnitType = VehicleUnitType,
        Status = Status,
        TagIds = TagIds.Distinct().ToList(),
    };
}

public class UpdateVehicleTypeRequestValidator : AbstractValidator<UpdateVehicleTypeRequest>
{
    public UpdateVehicleTypeRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Vehicle type ID");
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName();
        RuleFor(x => x.NameAr).IsRequiredArabicName();

        RuleFor(x => x.Category)
            .IsValidEnum<UpdateVehicleTypeRequest, VehicleTypeCategory>("Category");

        RuleFor(x => x.Status)
            .IsValidEnum<UpdateVehicleTypeRequest, VehicleTypeStatus>("Status");

        RuleFor(x => x.VehicleUnitType)
           .IsValidEnum<UpdateVehicleTypeRequest, VehicleUnitType>("VehicleUnitType")
               .When(x => x.VehicleUnitType.HasValue);

        RuleFor(x => x.TagIds)
            .IsRequiredNonEmptyCollection("TagIds");
    }
}
