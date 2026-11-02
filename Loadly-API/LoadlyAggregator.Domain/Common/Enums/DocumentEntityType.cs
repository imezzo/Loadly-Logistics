using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum DocumentEntityType
{
    [Display("Admin", "مسؤول")]
    ADMIN = 1,
    [Display("Company", "الشركات")]
    COMPANY = 2,
    [Display("Driver", "السائقين")]
    DRIVER = 3,
    [Display("Vehicle", "الاسطول")]
    VEHICLE = 4,
    [Display("Trip", "الرحلات")]
    TRIP = 5,
    [Display("Contract", "العقود")]
    CONTRACT = 6,
    [Display("Contractor", "المقاولين")]
    CONTRACTOR = 7,
    [Display("Contractor's Driver", "سائق المقاول")]
    CONTRACTOR_DRIVER = 8
}
