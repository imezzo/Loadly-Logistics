using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum VehicleFacilityType
{
    [Display("Garage", "جراج")]
    Garage = 1,

    [Display("Agency", "توكيل معتمد")]
    Agency = 2,

    [Display("Workshop", "ورشة")]
    Workshop = 3,

    [Display("Parking", "موقف")]
    Parking = 4
}
