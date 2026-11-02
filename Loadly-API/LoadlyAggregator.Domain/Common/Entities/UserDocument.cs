using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Entities;

public class UserDocument : BaseEntity
{
    public long DocumentId { get; set; }
    public string ModelType { get; set; } = string.Empty;
    public long ModelId { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DocumentVerificationStatus Status { get; set; } = DocumentVerificationStatus.PENDING;
    public long? AddedBy { get; set; }

    public virtual Document Document { get; set; } = null!;
}
