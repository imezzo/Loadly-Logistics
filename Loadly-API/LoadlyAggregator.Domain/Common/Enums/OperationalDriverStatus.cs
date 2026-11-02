using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum OperationalDriverStatus
{
    [Display("Heading to Pickup", "في الطريق للتحميل")]
    HeadingToPickup = 1,
    [Display("Loading In Progress", "جاري التحميل")]
    LoadingInProgress = 2,
    [Display("Heading to Dropoff", "في الطريق للتعتيق")]
    HeadingToDropoff = 3,
    [Display("Unloading In Progress", "جاري التعتيق")]
    UnloadingInProgress = 4,
    [Display("Enter Customs", "دخول الجمارك")]
    EnterCustoms = 5,
    [Display("Exit Customs", "خروج من الجمارك")]
    ExitCustoms = 6,

    [Display("Heading to Depot", "في الطريق للمستودع")]
    HeadingToDepot = 7,

    [Display("At Depot", "في المستودع")]
    AtDepot = 8
}
