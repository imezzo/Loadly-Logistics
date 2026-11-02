using LoadlyAggregator.Domain.Common.Attributes;
using LoadlyAggregator.Domain.Common.BaseClasses;
namespace LoadlyAggregator.Domain.Common.Entities;

[NoAudit]
public class LookupDocumentStatus : BaseEntity
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public long? AddedBy { get; set; }
}
