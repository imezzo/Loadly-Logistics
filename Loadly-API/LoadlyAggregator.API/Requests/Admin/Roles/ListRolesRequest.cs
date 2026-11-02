using LoadlyAggregator.Domain.Admin.DTOs.Roles;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Roles;

public class ListRolesRequest : BasePagedRequest
{
    public const string Permission = "manage_permissions";

    public string? GuardName { get; set; }

    public ListRolesDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        GuardName = GuardName
    };
}
