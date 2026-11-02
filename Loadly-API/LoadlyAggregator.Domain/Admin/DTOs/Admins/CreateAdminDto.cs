using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Admins;

public class CreateAdminDto
{
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int Status { get; set; } = 1;
    public int MfaChannel { get; set; } = (int)CommunicationChannel.None;
    public List<long>? RoleIds { get; set; }
}
