import '../database/app_database.dart';
import '../database/trip_gps_packet_segment.dart';
import '../database/trip_gps_trace_compute.dart';
import 'trip_gps_packet_policy.dart';
import 'trip_odometer_accumulation.dart';

/// Persists one trip's GPS packets and the driven-distance cursor.
///
/// Shared by the UI isolate and the Android foreground isolate so both apply
/// the same filter and distance rules.
class TripGpsTraceWriter {
  double totalMeters = 0;
  DateTime? lastPersistedAt;
  double? lastPersistedLat;
  double? lastPersistedLng;
  TripOdometerPoint? lastAccepted;

  void reset() {
    totalMeters = 0;
    lastPersistedAt = null;
    lastPersistedLat = null;
    lastPersistedLng = null;
    lastAccepted = null;
  }

  void seedFromPackets(List<GpsPacket> packets) {
    totalMeters = computeTotalMetersFromGpsPackets(packets);
    if (packets.isEmpty) {
      lastPersistedAt = null;
      lastPersistedLat = null;
      lastPersistedLng = null;
      lastAccepted = null;
      return;
    }
    final last = packets.last;
    lastPersistedAt = last.recordedAt;
    lastPersistedLat = last.latitude;
    lastPersistedLng = last.longitude;
    lastAccepted = TripOdometerPoint(
      last.latitude,
      last.longitude,
      last.recordedAt,
    );
  }

  /// Returns true when a row was inserted.
  Future<bool> record({
    required AppDatabase db,
    required int tripId,
    required double latitude,
    required double longitude,
    required DateTime timestamp,
    required double accuracyMeters,
    double? speedMps,
    double? headingDegrees,
    bool forcePersist = false,
    String source = 'live',
  }) async {
    final filteredSource = source == 'live' || source == 'background';
    if (!forcePersist && filteredSource) {
      final ok = TripGpsPacketPolicy.shouldPersistPacket(
        accuracyMeters: accuracyMeters,
        recordedAt: timestamp,
        latitude: latitude,
        longitude: longitude,
        lastPersistedAt: lastPersistedAt,
        lastPersistedLat: lastPersistedLat,
        lastPersistedLng: lastPersistedLng,
        speedMps: speedMps,
      );
      if (!ok) return false;
    } else if (accuracyMeters > TripGpsPacketPolicy.maxAccuracyMeters) {
      return false;
    }

    final segment = computeGpsPacketSegmentMetrics(
      latitude: latitude,
      longitude: longitude,
      previousLatitude: lastPersistedLat,
      previousLongitude: lastPersistedLng,
      deviceHeadingDegrees: headingDegrees,
      speedMps: speedMps,
    );

    await db.insertPacket(
      tripId: tripId,
      recordedAt: timestamp,
      latitude: latitude,
      longitude: longitude,
      accuracyM: accuracyMeters,
      speedMps: speedMps,
      headingDeg: segment.headingDegrees,
      distanceFromPreviousM: segment.distanceFromPreviousM,
      source: source,
    );

    lastPersistedAt = timestamp;
    lastPersistedLat = latitude;
    lastPersistedLng = longitude;

    final current = TripOdometerPoint(latitude, longitude, timestamp);
    final result = accumulateTripOdometerSegment(
      last: lastAccepted,
      current: current,
      accuracyMeters: accuracyMeters,
      speedMps: speedMps,
    );
    if (!result.accepted) return true;
    totalMeters += result.addedMeters;
    if (result.nextLast != null) {
      lastAccepted = result.nextLast;
    }
    return true;
  }
}
