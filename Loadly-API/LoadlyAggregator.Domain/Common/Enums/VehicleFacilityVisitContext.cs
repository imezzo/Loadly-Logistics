using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

/// <summary>
/// Distinguishes facility visit purpose. Service = workshop/garage (CurrentServiceFacilityId).
/// Parking = assigned home parking (AssignedParkingFacilityId) via parking enter/exit or assign API.
/// </summary>
public enum VehicleFacilityVisitContext
{
    [Display("Service", "خدمة")]
    Service = 1,
    [Display("Parking", "موقف")]
    Parking = 2
}
