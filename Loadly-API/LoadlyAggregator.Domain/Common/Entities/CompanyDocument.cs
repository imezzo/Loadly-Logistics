using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Entities;

public class CompanyDocument : BaseEntity
{
    public long DocumentId { get; set; }
    public long CompanyId { get; set; }
    public string? Path { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DocumentVerificationStatus Status { get; set; } = DocumentVerificationStatus.PENDING;
    public long? AddedBy { get; set; }

    public virtual Document Document { get; set; } = null!;
    public virtual Company Company { get; set; } = null!;
}
