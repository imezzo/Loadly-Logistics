namespace LoadlyAggregator.Domain.Admin.DTOs.Contracts;

public class PatchContractBillingSnapshotCalendarDayTripCountDto
{
    public long SnapshotId { get; set; }
    public long CalendarDayId { get; set; }
    public IReadOnlyList<long> TripIds { get; set; } = [];
    public string? Reason { get; set; }
}
