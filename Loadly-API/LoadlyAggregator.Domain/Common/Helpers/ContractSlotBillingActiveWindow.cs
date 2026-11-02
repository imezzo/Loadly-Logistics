namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ContractSlotBillingActiveWindow
{
    public static bool IsWorkingDayActiveForSlot(
        DateOnly date,
        DateOnly slotAssignedDate,
        DateOnly? slotArchivedDate,
        DateTime slotAssignedAtUtc,
        DateTime? slotArchivedAtUtc,
        ContractWorkingDayWindow workingDayWindow)
    {
        if (date < slotAssignedDate)
            return false;

        if (slotArchivedDate is not null && date > slotArchivedDate.Value)
            return false;

        var (dayStartUtc, dayEndUtc) = workingDayWindow.ToUtcRange(date);
        var windowEndUtc = slotArchivedAtUtc ?? DateTime.MaxValue;

        return slotAssignedAtUtc < dayEndUtc && dayStartUtc < windowEndUtc;
    }
}
