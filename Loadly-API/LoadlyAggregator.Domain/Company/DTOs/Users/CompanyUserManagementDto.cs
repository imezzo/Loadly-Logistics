using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Company.DTOs.Users;

public class CompanyUserManagementDto : BasePagedRequest
{
    public long? Id { get; set; }
    public long CompanyId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public CommunicationChannel MfaChannel { get; set; } = CommunicationChannel.None;
    public List<long>? RoleIds { get; set; }
    public int? Status { get; set; }
}
