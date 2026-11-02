using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ExtraFeeTypeDisplayNames
{
    private static readonly IReadOnlyDictionary<ExtraFeeTypes, (string NameEn, string NameAr)> Map = new Dictionary<ExtraFeeTypes, (string, string)>
    {
        [ExtraFeeTypes.TollGate] = ("Toll Gate", "كارتة مرور"),
        [ExtraFeeTypes.TollGateWeight] = ("Toll Gate Weight", "كارتة موازين"),
        [ExtraFeeTypes.OverWeight] = ("Over Weight Fee", "رسوم الوزن الإضافي"),
        [ExtraFeeTypes.HelpersFee] = ("Helpers Fee", "رسوم المساعدين"),
        [ExtraFeeTypes.WaitingTime] = ("Waiting Time", "وقت الانتظار"),
        [ExtraFeeTypes.LoadingUnloading] = ("Loading/Unloading", "التحميل/التفريغ"),
        [ExtraFeeTypes.MultiStop] = ("Multi-Stop", "توقف متعدد"),
        [ExtraFeeTypes.IncompleteReturn] = ("Incomplete Return", "إرجاع غير مكتمل"),
        [ExtraFeeTypes.Fuel] = ("Fuel", "الوقود"),
        [ExtraFeeTypes.OverNight] = ("Overnight Fee", "رسوم المبيت")
    };

    public static string GetNameEn(ExtraFeeTypes type) => Map.TryGetValue(type, out var t) ? t.NameEn : type.ToString();
    public static string GetNameAr(ExtraFeeTypes type) => Map.TryGetValue(type, out var t) ? t.NameAr : type.ToString();
    public static (string NameEn, string NameAr) Get(ExtraFeeTypes type) => Map.TryGetValue(type, out var t) ? t : (type.ToString(), type.ToString());
}
