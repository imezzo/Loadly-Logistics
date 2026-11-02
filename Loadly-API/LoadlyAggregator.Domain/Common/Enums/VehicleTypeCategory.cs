using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum VehicleTypeCategory
{
    [Display("Fridge", "ثلاجة")]
    Fridge = 0,

    [Display("Closed Box", "صندوق مغلق")]
    ClosedBox = 1,

    [Display("Vehicle Head", "رأس مركبة")]
    VehicleHead = 2,

    [Display("Flat Trailer", "مقطورة مسطحة")]
    FlatTrailer = 3,

    [Display("Pallet", "بالتة")]
    Pallet = 4,

    [Display("Trailer", "مقطورة")]
    Trailer = 5,

    [Display("Trailer With Side", "مقطورة جانبية")]
    TrailerWithSide = 6,

    [Display("Light Truck", "شاحنة خفيفة")]
    LightTruck = 7,
}
