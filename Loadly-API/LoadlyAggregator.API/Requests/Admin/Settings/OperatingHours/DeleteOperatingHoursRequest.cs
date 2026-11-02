using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.OperatingHours;

public class DeleteOperatingHoursRequest
{
    public const string Permission = "delete_operating_hour";

    public long Id { get; set; }

    public DeleteOperatingHoursDto FromRequest() => new()
    {
        Id = Id
    };
}
