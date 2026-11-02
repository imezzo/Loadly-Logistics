using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ContractSlotArchiveHelper
{
    public static DateTime ResolveArchivedAt(ContractSlot entity, DateTime? requestedArchivedAt)
    {
        var archivedAt = requestedArchivedAt ?? DateTime.UtcNow;
        ValidateArchivedAt(entity, archivedAt);
        return archivedAt;
    }

    public static void ValidateArchivedAt(ContractSlot entity, DateTime archivedAt)
    {
        if (archivedAt < entity.AssignedAt)
            throw new ValidationException("contracts.archived_at_before_assigned_at");

        if (archivedAt > DateTime.UtcNow)
            throw new ValidationException("contracts.archived_at_in_future");
    }
}
