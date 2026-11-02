using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Entities;

public class Address : BaseEntity
{
    public string ModelType { get; set; } = string.Empty;
    public long ModelId { get; set; }
    public string StreetName { get; set; } = string.Empty;
    public string? BuildingNumber { get; set; }
    public string? BuildingType { get; set; }
    public long GovernerateId { get; set; }
    public long CityId { get; set; }
    public long? DistrictId { get; set; }
    public string PostalCode { get; set; } = string.Empty;
    public AddressStatus Status { get; set; } = AddressStatus.ACTIVE;
    public long? AddedBy { get; set; }
}
