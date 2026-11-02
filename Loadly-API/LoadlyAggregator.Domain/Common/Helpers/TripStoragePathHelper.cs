namespace LoadlyAggregator.Domain.Common.Helpers;

public static class TripStoragePathHelper
{
    public static string OdometerScans(long tripId) => $"trips/{tripId}/odometer-scans";
    public static string OdometerStart(long tripId) => $"trips/{tripId}/odometer-start";
    public static string OdometerEnd(long tripId) => $"trips/{tripId}/odometer-end";
    public static string Tollgates(long tripId) => $"trips/{tripId}/tollgates";
    public static string TollgateWeights(long tripId) => $"trips/{tripId}/tollgate-weights";
    public static string ReceiptNote(long tripId, long waypointId) => $"trips/{tripId}/waypoints/{waypointId}/receipt-note";
    public static string ShipmentImages(long tripId, long waypointId) => $"trips/{tripId}/waypoints/{waypointId}/shipment-images";
    public static string DeliveryNote(long tripId, long waypointId) => $"trips/{tripId}/waypoints/{waypointId}/delivery-note";
    public static string UnloadingImages(long tripId, long waypointId) => $"trips/{tripId}/waypoints/{waypointId}/unloading-images";
    public static string UnloadingOdometer(long tripId, long waypointId) => $"trips/{tripId}/waypoints/{waypointId}/unloading-odometer";

    public static string CollectorDocument(long tripId, string slug) =>
        $"trips/{tripId}/collector/{slug}";

    public static string CustomsDocuments(long tripId) => $"trips/{tripId}/customs-documents";
    public static string CustomsReceipts(long tripId) => $"trips/{tripId}/customs-receipts";
}
