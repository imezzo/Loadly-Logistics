using LoadlyAggregator.Domain.Common.Attributes;
using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Entities;

[NoAudit]
public class Color : BaseEntity
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string ColorHash { get; set; } = string.Empty;
    public UserStatus Status { get; set; } = UserStatus.ACTIVE;
    public long? AddedBy { get; set; }
}
