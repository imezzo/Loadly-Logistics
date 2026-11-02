using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Settings;

public class ListVehicleBrandModelsRequest : BasePagedRequest
{
    public const string Permission = "view_vehicle_brand_model";

    public long? VehicleBrandId { get; set; }

    public ListVehicleBrandModelsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        VehicleBrandId = VehicleBrandId
    };
}

public class ListVehicleBrandModelsRequestValidator : AbstractValidator<ListVehicleBrandModelsRequest>
{
    public ListVehicleBrandModelsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
        RuleFor(x => x.VehicleBrandId)
            .IsOptionalId("Vehicle brand ID")
            .When(x => x.VehicleBrandId.HasValue);
    }
}
