using LoadlyAggregator.Domain.Common.Attributes;
using LoadlyAggregator.Domain.Common.BaseClasses;

namespace LoadlyAggregator.Domain.Common.Entities;

[NoAudit]
public class DeviceToken : BaseEntity
{
    public long UserId { get; set; }
    public string UserType { get; set; } = string.Empty;
    public string FcmToken { get; set; } = string.Empty;
    public string? DeviceId { get; set; }
    public string? DeviceModel { get; set; }
    public string? DeviceOs { get; set; }
    public string? AppVersion { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastUsedAt { get; set; }
}
