using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.VehicleBrands;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Settings.VehicleBrands;

public class ListVehicleBrandsRequest : BasePagedRequest
{
    public const string Permission = "view_vehicle_brand";


    public ListVehicleBrandsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search
    };
}

public class ListVehicleBrandsRequestValidator : AbstractValidator<ListVehicleBrandsRequest>
{
    public ListVehicleBrandsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
    }
}
