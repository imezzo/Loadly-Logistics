import 'package:flutter_test/flutter_test.dart';
import 'package:loadly_driver/core/location/trip_gps_packet_policy.dart';

void main() {
  test('rejects poor accuracy', () {
    expect(
      TripGpsPacketPolicy.shouldPersistPacket(
        accuracyMeters: 150,
        recordedAt: DateTime.utc(2026, 1, 1),
        latitude: 1,
        longitude: 2,
      ),
      isFalse,
    );
  });

  test('throttles urban idle samples', () {
    final t0 = DateTime.utc(2026, 1, 1, 12);
    expect(
      TripGpsPacketPolicy.shouldPersistPacket(
        accuracyMeters: 10,
        recordedAt: t0,
        latitude: 24,
        longitude: 46,
      ),
      isTrue,
    );
    expect(
      TripGpsPacketPolicy.shouldPersistPacket(
        accuracyMeters: 10,
        recordedAt: t0.add(const Duration(seconds: 5)),
        latitude: 24.00001,
        longitude: 46,
        lastPersistedAt: t0,
        lastPersistedLat: 24,
        lastPersistedLng: 46,
        speedMps: 0,
      ),
      isFalse,
    );
  });
}
