using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Entities;

public class VehicleDocument : BaseEntity
{
    public long DocumentId { get; set; }
    public long VehicleId { get; set; }
    public string? Path { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DocumentVerificationStatus Status { get; set; } = DocumentVerificationStatus.PENDING;
    public long? AddedBy { get; set; }
    public virtual Document Document { get; set; } = null!;
    public virtual Vehicle Vehicle { get; set; } = null!;
}
