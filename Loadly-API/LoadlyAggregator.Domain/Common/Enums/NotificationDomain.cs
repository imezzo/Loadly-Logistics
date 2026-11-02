namespace LoadlyAggregator.Domain.Common.Enums;

/// <summary>
/// Notification domains for queue routing and context.
/// Use enum for type safety; convert to string at serialization boundaries.
/// </summary>
public enum NotificationDomain
{
    Admin = 0,
    Company = 1,
    Driver = 2,
    Collector = 3,
    Contractor = 4,
    Default = 5
}
