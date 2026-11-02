using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class TripPlateDisplayHelper
{
    public static string FormatPlate(string? plateLetters, string? plateNo)
    {
        if (string.IsNullOrWhiteSpace(plateLetters) && string.IsNullOrWhiteSpace(plateNo))
            return string.Empty;

        return $"{plateLetters} {plateNo}".Trim();
    }

    public static string ResolveHeadDisplayPlate(Trip trip)
    {
        var snapshot = trip.VehicleAssignmentSnapshot;
        if (snapshot != null
            && (!string.IsNullOrWhiteSpace(snapshot.HeadPlateNo) || !string.IsNullOrWhiteSpace(snapshot.HeadPlateLetters)))
        {
            return FormatPlate(snapshot.HeadPlateLetters, snapshot.HeadPlateNo);
        }

        if (trip.Vehicle != null)
            return FormatPlate(trip.Vehicle.PlateLetters, trip.Vehicle.PlateNo);

        return string.Empty;
    }
}
