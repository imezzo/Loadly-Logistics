using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class TripChargeClassification
{
    public static bool IsStructural(TripChargeType chargeType) =>
        chargeType is TripChargeType.CargoWeight
            or TripChargeType.DestinationFixedPrice
            or TripChargeType.CargoCommission
            or TripChargeType.SpotPrice;
}
