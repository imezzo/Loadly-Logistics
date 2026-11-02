namespace LoadlyAggregator.Shared.Utilities;

public static class TripReferenceNumber
{
    public const string Prefix = "SH";

    public static string Format(long tripId) => $"{Prefix}{tripId}";

    public static bool IsTripDerivedValue(long tripId, string proofOfDelivery)
    {
        var pod = proofOfDelivery.Trim();
        if (pod.Length == 0)
            return false;

        var tripIdText = tripId.ToString();
        var referenceNumber = Format(tripId);

        if (pod.Equals(tripIdText, StringComparison.OrdinalIgnoreCase)
            || pod.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase))
            return true;

        return IsConcatenatedWith(pod, tripIdText) || IsConcatenatedWith(pod, referenceNumber);
    }

    private static bool IsConcatenatedWith(string pod, string token) =>
        pod.Length > token.Length
        && (pod.StartsWith(token, StringComparison.OrdinalIgnoreCase)
            || pod.EndsWith(token, StringComparison.OrdinalIgnoreCase));

}
