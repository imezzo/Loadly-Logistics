
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
namespace LoadlyAggregator.API.Requests.Admin.Drivers;

public class DeleteDriverRequest
{
    public const string Permission = "delete_driver";

    public long Id { get; set; }

    public DeleteDriverDto FromRequest() => new() { Id = Id };
}
