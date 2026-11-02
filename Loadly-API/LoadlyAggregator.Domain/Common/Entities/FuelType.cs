using LoadlyAggregator.Domain.Common.BaseClasses;
namespace LoadlyAggregator.Domain.Common.Entities;

public class FuelType : BaseEntity
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public long? AddedBy { get; set; }
}
