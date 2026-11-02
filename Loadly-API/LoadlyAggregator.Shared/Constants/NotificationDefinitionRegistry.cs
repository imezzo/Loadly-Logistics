using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Shared.Constants;

/// <summary>
/// Single source of truth for user-configurable notifications: event type, copy for UI, and which model types may receive each channel.
/// Add a new <see cref="NotificationDefinition"/> here, then hook senders to the same <see cref="NotificationDefinition.EventType"/>.
/// </summary>
public static class NotificationDefinitionRegistry
{
    public sealed record NotificationAudienceChannel(string ModelType, NotificationChannel Channel);

    /// <summary>Template keys for driver push copy when a trip moves to / updates scheduled state.</summary>
    public sealed record DriverTripScheduledPushTemplateKeys(
        string AssignedTitleTemplateKey,
        string AssignedBodyTemplateKey,
        string ScheduleUpdateTitleTemplateKey,
        string ScheduleUpdateBodyTemplateKey);

    /// <summary>Template keys for a single driver push event (e.g. trip cancelled).</summary>
    public sealed record DriverSimplePushTemplateKeys(
        string TitleTemplateKey,
        string BodyTemplateKey);

    public sealed record NotificationDefinition(
        NotificationEventType EventType,
        string DisplayNameEn,
        string DisplayNameAr,
        IReadOnlyList<NotificationAudienceChannel> AudienceChannels,
        string? CompanyEmailHeading = null,
        DriverTripScheduledPushTemplateKeys? DriverPushTemplateKeys = null,
        DriverSimplePushTemplateKeys? DriverCancelPushTemplateKeys = null);

    private static readonly NotificationDefinition[] Definitions =
    [
        new(
            NotificationEventType.TRIP_REQUESTED,
            "Trip requested",
            "طلب رحلة",
            [
                new NotificationAudienceChannel("Admin", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Admin", NotificationChannel.IN_APP)
            ]),
        new(
            NotificationEventType.TRIP_SCHEDULED,
            "Trip scheduled",
            "جدولة رحلة",
            [
                // new NotificationAudienceChannel("Company", NotificationChannel.EMAIL),
                new NotificationAudienceChannel("Company", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Company", NotificationChannel.IN_APP),
                new NotificationAudienceChannel("Driver", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Driver", NotificationChannel.IN_APP)
            ],
            CompanyEmailHeading: "Trip Scheduled",
            DriverPushTemplateKeys: new DriverTripScheduledPushTemplateKeys(
                AssignedTitleTemplateKey: "trip_assigned_title",
                AssignedBodyTemplateKey: "trip_assigned_body",
                ScheduleUpdateTitleTemplateKey: "trip_scheduled_title",
                ScheduleUpdateBodyTemplateKey: "trip_schedule_updated_body")),
        new(
            NotificationEventType.TRIP_CANCELLED,
            "Trip cancelled",
            "إلغاء رحلة",
            [
                // new NotificationAudienceChannel("Company", NotificationChannel.EMAIL),
                new NotificationAudienceChannel("Company", NotificationChannel.IN_APP),
                new NotificationAudienceChannel("Driver", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Driver", NotificationChannel.IN_APP),
                new NotificationAudienceChannel("Contractor", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Contractor", NotificationChannel.IN_APP)
            ],
            CompanyEmailHeading: "Trip Cancelled",
            DriverCancelPushTemplateKeys: new DriverSimplePushTemplateKeys(
                TitleTemplateKey: "trip_cancelled_driver_title",
                BodyTemplateKey: "trip_cancelled_driver_body")),
        new(
            NotificationEventType.TRIP_COMPLETED,
            "Trip completed",
            "اكتمال رحلة",
            [
                new NotificationAudienceChannel("Admin", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Admin", NotificationChannel.IN_APP),
                new NotificationAudienceChannel("Company", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Company", NotificationChannel.IN_APP)
            ]),
        new(
            NotificationEventType.TRIP_STARTED,
            "Trip started",
            "بدء الرحلة",
            [
                new NotificationAudienceChannel("Admin", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Admin", NotificationChannel.IN_APP),
                new NotificationAudienceChannel("Company", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Company", NotificationChannel.IN_APP)
            ]),
        new(
            NotificationEventType.TRIP_WAYPOINT_ARRIVED,
            "Arrived at waypoint",
            "وصول إلى نقطة الطريق",
            [
                new NotificationAudienceChannel("Admin", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Admin", NotificationChannel.IN_APP),
                new NotificationAudienceChannel("Company", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Company", NotificationChannel.IN_APP)
            ]),
        new(
            NotificationEventType.TRIP_WAYPOINT_LOADING_STARTED,
            "Loading started",
            "بدء التحميل",
            [
                new NotificationAudienceChannel("Admin", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Admin", NotificationChannel.IN_APP),
                new NotificationAudienceChannel("Company", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Company", NotificationChannel.IN_APP)
            ]),
        new(
            NotificationEventType.TRIP_WAYPOINT_LOADING_COMPLETED,
            "Loading completed",
            "اكتمال التحميل",
            [
                new NotificationAudienceChannel("Admin", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Admin", NotificationChannel.IN_APP),
                new NotificationAudienceChannel("Company", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Company", NotificationChannel.IN_APP)
            ]),
        new(
            NotificationEventType.TRIP_WAYPOINT_UNLOADING_STARTED,
            "Unloading started",
            "بدء التفريغ",
            [
                new NotificationAudienceChannel("Admin", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Admin", NotificationChannel.IN_APP),
                new NotificationAudienceChannel("Company", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Company", NotificationChannel.IN_APP)
            ]),
        new(
            NotificationEventType.TRIP_WAYPOINT_UNLOADING_COMPLETED,
            "Unloading completed",
            "اكتمال التفريغ",
            [
                new NotificationAudienceChannel("Admin", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Admin", NotificationChannel.IN_APP),
                new NotificationAudienceChannel("Company", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Company", NotificationChannel.IN_APP)
            ]),
        new(
            NotificationEventType.TRIP_WAYPOINT_DEPARTED,
            "Departed from waypoint",
            "مغادرة نقطة الطريق",
            [
                new NotificationAudienceChannel("Admin", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Admin", NotificationChannel.IN_APP),
                new NotificationAudienceChannel("Company", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Company", NotificationChannel.IN_APP)
            ]),
        new(
            NotificationEventType.TRIP_PUSHED_TO_CONTRACTORS,
            "Trip pushed to contractors",
            "إرسال رحلة للمقاولين",
            [
                new NotificationAudienceChannel("Contractor", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Contractor", NotificationChannel.IN_APP)
            ]),
        new(
            NotificationEventType.TRIP_BOOKING_REQUEST_SUBMITTED,
            "Trip booking request submitted",
            "تقديم طلب حجز رحلة",
            [
                new NotificationAudienceChannel("Admin", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Admin", NotificationChannel.IN_APP)
            ]),
        new(
            NotificationEventType.TRIP_BOOKING_REQUEST_APPROVED,
            "Trip booking request approved",
            "الموافقة على طلب حجز رحلة",
            [
                new NotificationAudienceChannel("Contractor", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Contractor", NotificationChannel.IN_APP)
            ]),
        new(
            NotificationEventType.TRIP_CONTRACTOR_UNASSIGNED,
            "Unassigned from trip",
            "إلغاء التعيين للرحلة",
            [
                new NotificationAudienceChannel("Contractor", NotificationChannel.PUSH),
                new NotificationAudienceChannel("Contractor", NotificationChannel.IN_APP)
            ]),
    ];

    public static IReadOnlyList<NotificationDefinition> All => Definitions;

    private static readonly Dictionary<NotificationEventType, NotificationDefinition> ByEventType =
        Definitions.ToDictionary(d => d.EventType);

    public static NotificationDefinition? TryGetDefinition(NotificationEventType eventType) =>
        ByEventType.TryGetValue(eventType, out var def) ? def : null;

    public static string GetCompanyHeadingOrDisplayName(NotificationEventType eventType)
    {
        NotificationDefinition? d = TryGetDefinition(eventType);
        if (d is null)
            return eventType.ToString();
        return d.CompanyEmailHeading ?? d.DisplayNameEn;
    }

    public static bool IsPreferenceConfigurable(string normalizedModelType, NotificationChannel channel, NotificationEventType eventType)
    {
        if (string.IsNullOrWhiteSpace(normalizedModelType))
            return false;
        return Definitions.Any(d =>
            d.EventType == eventType
            && d.AudienceChannels.Any(r =>
                r.ModelType.Equals(normalizedModelType, StringComparison.OrdinalIgnoreCase)
                && r.Channel == channel));
    }

    public static IReadOnlyDictionary<string, IReadOnlyList<NotificationPreferenceCatalog.Entry>> BuildPreferenceCatalogByModelType()
    {
        Dictionary<string, List<NotificationPreferenceCatalog.Entry>> buckets = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Admin"] = [],
            ["Company"] = [],
            ["Driver"] = [],
            ["Contractor"] = []
        };

        foreach (NotificationDefinition def in Definitions)
        {
            foreach (NotificationAudienceChannel rule in def.AudienceChannels)
            {
                if (!buckets.TryGetValue(rule.ModelType, out List<NotificationPreferenceCatalog.Entry>? list))
                    continue;
                list.Add(new NotificationPreferenceCatalog.Entry(def.EventType, rule.Channel));
            }
        }

        return buckets.ToDictionary(
            static kv => kv.Key,
            static kv => (IReadOnlyList<NotificationPreferenceCatalog.Entry>)kv.Value.AsReadOnly(),
            StringComparer.OrdinalIgnoreCase);
    }
}
