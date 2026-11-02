using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Settings;

public class ListVehicleInsurancesRequest : BasePagedRequest
{
    public const string Permission = "view_vehicle_insurance";


    public ListVehicleInsurancesDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search
    };
}

public class ListVehicleInsurancesRequestValidator : AbstractValidator<ListVehicleInsurancesRequest>
{
    public ListVehicleInsurancesRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
    }
}
