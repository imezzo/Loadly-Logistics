
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
namespace LoadlyAggregator.API.Requests.Admin.Drivers;

public class GetDriverRequest
{
    public const string Permission = "view_driver";

    public long Id { get; set; }

    public GetDriverDto FromRequest() => new() { Id = Id };
}
