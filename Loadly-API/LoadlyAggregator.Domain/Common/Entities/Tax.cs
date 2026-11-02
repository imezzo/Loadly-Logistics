using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Entities;

public class Tax : BaseEntity
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public TaxType Type { get; set; }
    public decimal Value { get; set; }
    public TaxStatus Status { get; set; } = TaxStatus.ACTIVE;
    public long? AddedBy { get; set; }
}
