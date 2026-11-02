using LoadlyAggregator.Domain.Common.BaseClasses;
namespace LoadlyAggregator.Domain.Common.Entities;

public class City : BaseEntity
{
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public long GovernerateId { get; set; }
    public long? AddedBy { get; set; }

    public virtual Governerate Governerate { get; set; } = null!;
}
