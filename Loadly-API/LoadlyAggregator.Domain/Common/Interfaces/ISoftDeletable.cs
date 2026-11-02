namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ISoftDeletable
{
    DateTime? DeletedAt { get; set; }
    bool IsDeleted { get; }
}

public static class SoftDeletableExtensions
{
    public static void SetDeletedAt(this ISoftDeletable entity, DateTime timestamp)
    {
        entity.DeletedAt = timestamp;
    }
}
