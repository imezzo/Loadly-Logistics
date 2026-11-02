using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum PaymentMethod
{
    [Display("Bank Transfer", "تحويل بنكي")]
    BankTransfer = 1,
    [Display("Cheque", "شيك")]
    Cheque = 2,
    [Display("Credit", "بطاقة بنكية")]
    Credit = 3
}
