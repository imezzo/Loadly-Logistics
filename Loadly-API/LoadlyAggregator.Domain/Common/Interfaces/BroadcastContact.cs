namespace LoadlyAggregator.Domain.Common.Interfaces;

/// <summary>
/// Contact information for broadcast notifications.
/// </summary>
public class BroadcastContact
{
    public long ModelId { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
}
