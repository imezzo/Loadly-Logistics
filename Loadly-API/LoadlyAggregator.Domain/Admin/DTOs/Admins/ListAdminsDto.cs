using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.DTOs.Admins;

public class ListAdminsDto : BasePagedRequest
{
    public int? Type { get; set; }
    public int? Status { get; set; }
    public List<long>? RoleIds { get; set; }
}
