using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Settings.FuelTypes;

public class ListFuelTypesRequest : BasePagedRequest
{
    public const string Permission = "view_fuel_type";


    public ListFuelTypesDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search
    };
}
