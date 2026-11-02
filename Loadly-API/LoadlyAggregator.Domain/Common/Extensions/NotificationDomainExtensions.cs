using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Extensions;

public static class NotificationDomainExtensions
{
    public static string ToStringValue(this NotificationDomain domain) =>
        domain switch
        {
            NotificationDomain.Admin => "admin",
            NotificationDomain.Company => "company",
            NotificationDomain.Driver => "driver",
            NotificationDomain.Contractor => "contractor",
            NotificationDomain.Collector => "collector",
            NotificationDomain.Default => "default",
            _ => domain.ToString().ToLowerInvariant()
        };

    public static NotificationDomain ToNotificationDomain(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return NotificationDomain.Default;

        return value.ToLowerInvariant() switch
        {
            "admin" => NotificationDomain.Admin,
            "company" => NotificationDomain.Company,
            "driver" => NotificationDomain.Driver,
            "contractor" => NotificationDomain.Contractor,
            "collector" => NotificationDomain.Collector,
            "default" => NotificationDomain.Default,
            _ => NotificationDomain.Default
        };
    }
}
