using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Entities;

public class DriverLicenseType : BaseEntity
{
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public long? AddedBy { get; set; }
    public UserStatus Status { get; set; } = UserStatus.ACTIVE;
}
