using LoadlyAggregator.Domain.Common.BaseClasses;
namespace LoadlyAggregator.Domain.Common.Entities;

public class District : BaseEntity
{
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public long CityId { get; set; }
    public long? AddedBy { get; set; }

    public virtual City City { get; set; } = null!;
}
