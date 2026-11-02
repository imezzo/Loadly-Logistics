using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum TollGateCalculation
{
    [Display("Billed In Trip Cost", "مُضافة للفاتورة")]
    Included = 1,
    [Display("Excluded from Invoice", "خارج الفاتورة")]
    ClientPay = 2,
    [Display("Fixed amount per trip", "مبلغ ثابت لكل رحلة")]
    Fixed = 3,
    [Display("Variable Per Route", "حسب مسار الرحلة")]
    VariablePerRoute = 4
}
