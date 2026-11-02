using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class Document : BaseEntity
{
    public DocumentEntityType EntityId { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public bool Required { get; set; } = false;
    public DocumentStatus Status { get; set; } = DocumentStatus.ACTIVE;
    public long? AddedBy { get; set; }
}
