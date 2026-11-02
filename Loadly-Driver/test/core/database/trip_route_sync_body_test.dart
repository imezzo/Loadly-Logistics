import 'package:flutter_test/flutter_test.dart';
import 'package:loadly_driver/core/database/app_database.dart';
import 'package:loadly_driver/core/database/trip_route_sync_body.dart';
import 'package:drift/native.dart';

void main() {
  test(
    'buildTripRouteSyncBody includes encoded route and session fields',
    () async {
      final db = AppDatabase.forTesting(NativeDatabase.memory());
      const tripId = 42;
      await db.beginTripSession(tripId);
      final t0 = DateTime.utc(2026, 10, 13, 10, 0, 0);
      await db.insertPacket(
        tripId: tripId,
        recordedAt: t0,
        latitude: 30.01022,
        longitude: 31.235,
        accuracyM: 5,
        speedMps: 0,
        headingDeg: 0,
        distanceFromPreviousM: 0,
      );
      await db.endSession(tripId: tripId, totalMeters: 1250.5);

      final payload = await db.buildUploadPayload(tripId);
      expect(payload, isNotNull);

      final body = buildTripRouteSyncBody(payload!);
      expect(body['distanceMeters'], 1251);
      expect(body['gpsPacketCount'], 1);
      expect((body['route'] as Map)['n'], 1);
      expect(body['startTime'], payload.startedAt.toUtc().toIso8601String());
      expect(body['endTime'], isNotNull);

      final route = body['route'] as Map<String, dynamic>;
      expect(route['n'], 1);
      expect(route['t0'], t0.millisecondsSinceEpoch ~/ 1000);
      await db.close();
    },
  );

  test('gpsPacketCount uses dense count after per-second dedupe', () async {
    final db = AppDatabase.forTesting(NativeDatabase.memory());
    const tripId = 99;
    await db.beginTripSession(tripId);
    final t0 = DateTime.utc(2026, 10, 13, 10, 0, 0);
    await db.insertPacket(
      tripId: tripId,
      recordedAt: t0,
      latitude: 30.0,
      longitude: 31.0,
      accuracyM: 5,
      speedMps: 0,
      distanceFromPreviousM: 0,
    );
    await db.insertPacket(
      tripId: tripId,
      recordedAt: t0.add(const Duration(milliseconds: 500)),
      latitude: 30.001,
      longitude: 31.001,
      accuracyM: 5,
      speedMps: 1,
      distanceFromPreviousM: 100,
    );
    await db.endSession(tripId: tripId, totalMeters: 100);

    final payload = await db.buildUploadPayload(tripId);
    final body = buildTripRouteSyncBody(payload!);

    expect(payload.packets.length, 2);
    expect(body['gpsPacketCount'], 1);
    expect((body['route'] as Map)['n'], 1);

    await db.close();
  });
}
