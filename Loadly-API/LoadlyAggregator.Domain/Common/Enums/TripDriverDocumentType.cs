using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

/// <summary>
/// Driver-uploaded trip media types derived from recordings.
/// </summary>
public enum TripDriverDocumentType
{
    [Display("Odometer Start", "قراءة العداد عند البدء")]
    OdometerStart,

    [Display("Odometer End", "قراءة العداد عند الانتهاء")]
    OdometerEnd,

    [Display("Receipt Note", "إذن الاستلام")]
    ReceiptNote,

    [Display("Shipment Images", "صور الشحنة")]
    ShipmentImages,

    [Display("Delivery Note", "إذن التسليم")]
    DeliveryNote,

    [Display("Unloading Images", "صور التفريغ")]
    UnloadingImages,

    [Display("Unloading Odometer", "قراءة العداد بعد اكتمال التعتيق")]
    UnloadingOdometer,

    [Display("Tollgate Images", "صور الكارتة")]
    TollgateImages,

    [Display("Tollgate Weight Images", "صور كارتة الموازين")]
    TollgateWeightImages,

    [Display("Customs Documents", "مستندات الجمارك")]
    CustomsDocuments,

    [Display("Customs Receipt", "إيصال الجمارك")]
    CustomsReceipt
}
