using LoadlyAggregator.Domain.Common.Attributes;
using LoadlyAggregator.Domain.Common.BaseClasses;
namespace LoadlyAggregator.Domain.Common.Entities;

[NoAudit]
public class RefreshToken : BaseEntity
{
    public string TokenHash { get; set; } = string.Empty;
    public long ModelId { get; set; }
    public string ModelType { get; set; } = string.Empty;
    public string TokenFamilyId { get; set; } = string.Empty;
    public string? DeviceInfo { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public string? SessionName { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByTokenHash { get; set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsExpired && !IsRevoked;
}
