using System.Text.Json;
using LoadlyAggregator.Domain.Common.Attributes;
using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

[NoAudit]
public class Recording : BaseEntity
{
    public RecordableType RecordableType { get; set; }
    public long RecordableId { get; set; }
    public RecordableEventType RecordableEventType { get; set; }
    public long? AddedBy { get; set; }
    public string? AddedByModelType { get; set; }
    public JsonDocument? Data { get; set; }
}
