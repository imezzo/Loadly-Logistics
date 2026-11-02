import 'package:geolocator/geolocator.dart';

/// Filters which GPS fixes are persisted (matches distance quality rules).
abstract final class TripGpsPacketPolicy {
  static const maxAccuracyMeters = 100.0;

  static const highwaySpeedMps = 30 / 3.6; // 30 km/h

  static const highwayMinInterval = Duration(seconds: 10);
  static const highwayMinMoveMeters = 15.0;

  static const urbanMinInterval = Duration(seconds: 30);
  static const urbanMinMoveMeters = 25.0;

  /// Returns false when the fix should not be stored in SQLite.
  static bool shouldPersistPacket({
    required double accuracyMeters,
    required DateTime recordedAt,
    required double latitude,
    required double longitude,
    DateTime? lastPersistedAt,
    double? lastPersistedLat,
    double? lastPersistedLng,
    double? speedMps,
  }) {
    if (accuracyMeters > maxAccuracyMeters) {
      return false;
    }

    if (lastPersistedAt == null ||
        lastPersistedLat == null ||
        lastPersistedLng == null) {
      return true;
    }

    final elapsed = recordedAt.difference(lastPersistedAt);
    final moved = Geolocator.distanceBetween(
      lastPersistedLat,
      lastPersistedLng,
      latitude,
      longitude,
    );

    final highway = speedMps != null && speedMps >= highwaySpeedMps;
    final minInterval = highway ? highwayMinInterval : urbanMinInterval;
    final minMove = highway ? highwayMinMoveMeters : urbanMinMoveMeters;

    if (elapsed < minInterval && moved < minMove) {
      return false;
    }

    return true;
  }
}
