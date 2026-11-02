using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class TripDocument : BaseEntity
{
    public long TripId { get; set; }
    public long DocumentId { get; set; }
    public string[]? Paths { get; set; }
    public CollectorDocumentStatus? Status { get; set; }
    public string? Note { get; set; }
    public long? AddedBy { get; set; }

    public virtual Trip Trip { get; set; } = null!;
    public virtual Document Document { get; set; } = null!;
}
