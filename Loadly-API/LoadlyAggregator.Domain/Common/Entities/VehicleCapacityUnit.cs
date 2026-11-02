using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Entities;

public class VehicleCapacityUnit : BaseEntity
{
    public string CapacityTypeEn { get; set; } = string.Empty;
    public string CapacityTypeAr { get; set; } = string.Empty;
    public UserStatus Status { get; set; } = UserStatus.ACTIVE;
    public long? AddedBy { get; set; }
}
