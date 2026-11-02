import 'package:drift/native.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:loadly_driver/core/database/app_database.dart';
import 'package:loadly_driver/core/database/trip_gps_trace_compute.dart';

void main() {
  late AppDatabase db;

  setUp(() {
    db = AppDatabase.forTesting(NativeDatabase.memory());
  });

  tearDown(() async {
    await db.close();
  });

  test('computeTotalMetersFromGpsPackets sums stored path', () async {
    const tripId = 1;
    await db.beginTripSession(tripId);

    var lat = 24.0;
    const lng = 46.0;
    var t = DateTime.utc(2026, 1, 1, 12);
    for (var i = 0; i < 20; i++) {
      lat += 0.0005;
      t = t.add(const Duration(seconds: 20));
      await db.insertPacket(
        tripId: tripId,
        recordedAt: t,
        latitude: lat,
        longitude: lng,
        accuracyM: 10,
        speedMps: 2.5,
      );
    }

    final packets = await db.packetsForTrip(tripId);
    final meters = computeTotalMetersFromGpsPackets(packets);
    expect(meters, greaterThan(500));
  });
}
