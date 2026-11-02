using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.VehicleTypes;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Settings.VehicleTypes;

public class ListVehicleTypesRequest : BasePagedRequest
{
    public const string Permission = "view_vehicle_type";

    public VehicleTypeCategory? Category { get; set; }
    public List<VehicleUnitType>? VehicleUnitTypes { get; set; }
    public VehicleTypeStatus? Status { get; set; }

    public ListVehicleTypesDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        Category = Category,
        VehicleUnitTypes = VehicleUnitTypes,
        Status = Status
    };
}

public class ListVehicleTypesRequestValidator : AbstractValidator<ListVehicleTypesRequest>
{
    public ListVehicleTypesRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
    }
}
