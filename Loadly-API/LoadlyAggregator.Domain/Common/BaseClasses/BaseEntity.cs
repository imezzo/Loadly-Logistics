using LoadlyAggregator.Domain.Common.Interfaces;
namespace LoadlyAggregator.Domain.Common.BaseClasses;

public abstract class BaseEntity : IEntity, IAuditable, ISoftDeletable
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted => DeletedAt != null;
}
