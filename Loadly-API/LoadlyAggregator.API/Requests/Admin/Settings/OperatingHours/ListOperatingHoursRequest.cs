using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Settings.OperatingHours;

public class ListOperatingHoursRequest : BasePagedRequest
{
    public const string Permission = "view_operating_hour";


    public ListOperatingHoursDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search
    };
}
