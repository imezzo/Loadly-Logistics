using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum TripChargeType
{
    // Contract vehicle type based costs
    [Display("Cargo Weight", "وزن الشحن")]
    CargoWeight = 5,
    [Display("Destination Fixed Price", "السعر الثابت للوجهة")]
    DestinationFixedPrice = 6,
    [Display("Cargo Commission", "عمولة على الشحنة")]
    CargoCommission = 7,
    [Display("Spot Price", "السعر الفوري")]
    SpotPrice = 8,
    [Display("OverNight", "مبيت")]
    OverNight = 18,
    [Display("Customs", "الجمارك")]
    Customs = 19,
    // Contract extra fee based costs
    [Display("Fuel", "الوقود")]
    Fuel = 2,
    [Display("Toll Gate", "رسوم كارتة مرور")]
    TollGate = 10,
    [Display("Over Weight Charge", "رسوم الوزن الإضافي")]
    OverWeight = 11,
    [Display("Helpers Fee", "رسوم المساعدين")]
    HelpersFee = 12,
    [Display("Waiting Time", "وقت الانتظار")]
    WaitingTime = 13,
    [Display("Loading/Unloading", "التحميل/التفريغ")]
    LoadingUnloading = 14,
    [Display("Multi Stop", "محطات متعددة")]
    MultiStop = 15,
    [Display("Incomplete Return", "العودة غير المكتملة")]
    IncompleteReturn = 16,
    [Display("Toll Gate Weight", "رسوم كارتة موازين")]
    TollGateWeight = 17
}
