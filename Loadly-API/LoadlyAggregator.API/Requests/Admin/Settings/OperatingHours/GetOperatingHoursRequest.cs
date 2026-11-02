using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.OperatingHours;

public class GetOperatingHoursRequest
{
    public const string Permission = "view_operating_hour";

    public long Id { get; set; }

    public GetOperatingHoursDto FromRequest() => new()
    {
        Id = Id
    };
}
