import 'package:drift/native.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:loadly_driver/core/database/app_database.dart';
import 'package:loadly_driver/core/database/trip_route_codec.dart';
import 'package:loadly_driver/core/database/trip_route_downsample.dart';
import 'package:loadly_driver/core/database/trip_route_sync_body.dart';

void main() {
  group('prepareTripRoutePointsForUpload', () {
    test('keeps all points when count is within max', () {
      final points = List.generate(
        10,
        (i) => TripRouteCodecPoint(
          t: 1_700_000_000 + i * 60,
          lat: 24.0 + i * 0.001,
          lng: 46.0,
          spd: 50,
          hdg: 90,
          acc: 10,
          dsm: 999,
        ),
      );

      final upload = prepareTripRoutePointsForUpload(points);
      expect(upload.length, 10);
      expect(upload.first.t, points.first.t);
      expect(upload.last.t, points.last.t);
      expect(upload.first.dsm, 0);
      expect(upload[1].dsm, greaterThan(0));
      expect(upload[1].dsm, isNot(999));
    });

    test('downsamples long trace to at most 30 with first and last kept', () {
      final points = List.generate(
        500,
        (i) => TripRouteCodecPoint(
          t: 1_700_000_000 + i * 120,
          lat: 24.0 + i * 0.0001,
          lng: 46.0 + i * 0.0001,
          spd: 60,
          hdg: 45,
          acc: 8,
          dsm: 50,
        ),
      );

      final upload = prepareTripRoutePointsForUpload(points);
      expect(upload.length, lessThanOrEqualTo(30));
      expect(upload.length, greaterThan(2));
      expect(upload.first.t, points.first.t);
      expect(upload.last.t, points.last.t);

      for (var i = 1; i < upload.length; i++) {
        expect(upload[i].t, greaterThanOrEqualTo(upload[i - 1].t));
        expect(upload[i].dsm, greaterThan(0));
        expect(upload[i].t - upload[i - 1].t, greaterThan(0));
      }
    });

    test('encodeTripRouteFromPackets applies downsampling on sync path', () async {
      final db = AppDatabase.forTesting(NativeDatabase.memory());
      const tripId = 7;
      await db.beginTripSession(tripId);
      final t0 = DateTime.utc(2026, 6, 1, 8);
      for (var i = 0; i < 200; i++) {
        await db.insertPacket(
          tripId: tripId,
          recordedAt: t0.add(Duration(minutes: i * 15)),
          latitude: 24.0 + i * 0.01,
          longitude: 46.0 + i * 0.01,
          accuracyM: 5,
          speedMps: 15,
          headingDeg: 90,
          distanceFromPreviousM: 1000,
        );
      }
      await db.endSession(tripId: tripId, totalMeters: 199_000);

      final payload = await db.buildUploadPayload(tripId);
      expect(payload, isNotNull);

      final body = buildTripRouteSyncBody(payload!);
      expect(body['distanceMeters'], 199_000);
      expect(body['gpsPacketCount'], 200);

      final route = body['route'] as Map<String, dynamic>;
      expect(route['n'], lessThanOrEqualTo(30));
      expect(body['gpsPacketCount'], greaterThan(route['n'] as int));
      expect((route['dt'] as List).length, lessThanOrEqualTo(30));

      await db.close();
    });
  });
}
