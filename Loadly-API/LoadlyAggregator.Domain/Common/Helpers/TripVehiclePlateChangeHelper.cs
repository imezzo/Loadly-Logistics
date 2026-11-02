using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class TripVehiclePlateChangeHelper
{
    public static bool IsHeadVehiclePlateChanged(Trip trip)
    {
        var snapshot = trip.VehicleAssignmentSnapshot;
        if (snapshot is null)
            return false;

        if (trip.Vehicle is null)
            return HasPlatePair(snapshot.HeadPlateNo, snapshot.HeadPlateLetters);

        return !PlatePairsEqual(
            snapshot.HeadPlateNo,
            snapshot.HeadPlateLetters,
            trip.Vehicle.PlateNo,
            trip.Vehicle.PlateLetters);
    }

    public static bool IsTailVehiclePlateChanged(Trip trip)
    {
        var snapshot = trip.VehicleAssignmentSnapshot;
        if (snapshot is null)
            return false;

        if (trip.TailVehicle is null)
            return HasPlatePair(snapshot.TailPlateNo, snapshot.TailPlateLetters);

        return !PlatePairsEqual(
            snapshot.TailPlateNo ?? string.Empty,
            snapshot.TailPlateLetters ?? string.Empty,
            trip.TailVehicle.PlateNo,
            trip.TailVehicle.PlateLetters);
    }

    /// <summary>
    /// When the head plate on the assigned vehicle first diverged from the trip snapshot (supersede time of the snapshot plate).
    /// </summary>
    public static DateTime? GetHeadPlateChangedAt(Trip trip)
    {
        if (!IsHeadVehiclePlateChanged(trip))
            return null;

        var snapshot = trip.VehicleAssignmentSnapshot!;
        return FindSupersededAtForSnapshotPlate(
            trip.Vehicle?.PlateAliases,
            snapshot.HeadPlateNo,
            snapshot.HeadPlateLetters);
    }

    /// <summary>
    /// When the tail plate on the assigned tail vehicle first diverged from the trip snapshot (supersede time of the snapshot plate).
    /// </summary>
    public static DateTime? GetTailPlateChangedAt(Trip trip)
    {
        if (!IsTailVehiclePlateChanged(trip))
            return null;

        var snapshot = trip.VehicleAssignmentSnapshot!;
        return FindSupersededAtForSnapshotPlate(
            trip.TailVehicle?.PlateAliases,
            snapshot.TailPlateNo ?? string.Empty,
            snapshot.TailPlateLetters ?? string.Empty);
    }

    private static DateTime? FindSupersededAtForSnapshotPlate(
        ICollection<VehiclePlateAlias>? aliases,
        string snapshotPlateNo,
        string? snapshotPlateLetters)
    {
        if (aliases is null || aliases.Count == 0)
            return null;

        var alias = aliases.FirstOrDefault(a => PlatePairsEqual(
            snapshotPlateNo,
            snapshotPlateLetters ?? string.Empty,
            a.PlateNo,
            a.PlateLetters));

        return alias?.CreatedAt;
    }

    private static bool HasPlatePair(string? plateNo, string? plateLetters) =>
        !string.IsNullOrWhiteSpace(plateNo) || !string.IsNullOrWhiteSpace(plateLetters);

    private static bool PlatePairsEqual(string plateNoA, string plateLettersA, string plateNoB, string plateLettersB) =>
        string.Equals(plateNoA.Trim(), plateNoB.Trim(), StringComparison.OrdinalIgnoreCase)
        && string.Equals(plateLettersA.Trim(), plateLettersB.Trim(), StringComparison.OrdinalIgnoreCase);
}
