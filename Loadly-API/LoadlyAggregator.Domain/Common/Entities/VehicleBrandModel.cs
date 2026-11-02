using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Entities;

public class VehicleBrandModel : BaseEntity
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public long VehicleBrandId { get; set; }
    public UserStatus Status { get; set; } = UserStatus.ACTIVE;
    public long? AddedBy { get; set; }

    public virtual VehicleBrand VehicleBrand { get; set; } = null!;
}
