import 'package:drift/native.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:loadly_driver/core/database/app_database.dart';
import 'package:loadly_driver/core/database/trip_route_codec.dart';

void main() {
  group('encodeTripRoute', () {
    test('matches column layout for three sample points (spd km/h)', () {
      final encoded = encodeTripRoute([
        const TripRouteCodecPoint(
          t: 1728813600,
          lat: 30.01022,
          lng: 31.235,
          spd: 0,
          hdg: 0,
          acc: 5,
          dsm: 0,
        ),
        const TripRouteCodecPoint(
          t: 1728813605,
          lat: 30.01023,
          lng: 31.23501,
          spd: 45,
          hdg: 90,
          acc: 4,
          dsm: 420,
        ),
        const TripRouteCodecPoint(
          t: 1728813610,
          lat: 30.01024,
          lng: 31.23502,
          spd: 50,
          hdg: 95,
          acc: 3.5,
          dsm: 430,
        ),
      ]);

      expect(encoded.n, 3);
      expect(encoded.t0, 1728813600);
      expect(encoded.dt, [0, 5, 5]);
      expect(encoded.lat, [3001022, 1, 1]);
      expect(encoded.lng, [3123500, 1, 1]);
      expect(encoded.spd, [0, 45, 5]);
      expect(encoded.hdg, [0, 90, 95]);
      expect(encoded.acc, [5, 4, 4]);
      expect(encoded.dsm, [0, 420, 10]);
    });

    test('converts m/s from packets to km/h in route', () async {
      final db = AppDatabase.forTesting(NativeDatabase.memory());
      await db.beginTripSession(1);
      await db.insertPacket(
        tripId: 1,
        recordedAt: DateTime.utc(2026, 1, 1),
        latitude: 0,
        longitude: 0,
        accuracyM: 1,
        speedMps: 10,
        distanceFromPreviousM: 0,
      );
      final points = tripRoutePointsFromGpsPackets(await db.packetsForTrip(1));
      expect(points.single.spd, 36);
      await db.close();
    });

    test('round-trips through decode', () {
      const input = [
        TripRouteCodecPoint(
          t: 1728813600,
          lat: 24.7136,
          lng: 46.6753,
          spd: 44.64,
          hdg: 180,
          acc: 8.2,
          dsm: 0,
        ),
        TripRouteCodecPoint(
          t: 1728813610,
          lat: 24.714,
          lng: 46.676,
          spd: 54.36,
          hdg: 175,
          acc: 6,
          dsm: 55.3,
        ),
      ];

      final encoded = encodeTripRoute(input);
      final decoded = _decodeTripRoute(encoded);

      expect(decoded.length, input.length);
      for (var i = 0; i < input.length; i++) {
        expect(decoded[i].t, input[i].t);
        expect(decoded[i].lat, closeTo(input[i].lat, 1e-5));
        expect(decoded[i].lng, closeTo(input[i].lng, 1e-5));
        expect(decoded[i].spd, closeTo(input[i].spd.round(), 0.01));
        expect(decoded[i].hdg, input[i].hdg);
        expect(decoded[i].acc, closeTo(input[i].acc.round(), 0.01));
        expect(decoded[i].dsm, closeTo(input[i].dsm.round(), 0.01));
      }
    });
  });
}

List<TripRouteCodecPoint> _decodeTripRoute(EncodedTripRoute p) {
  final result = <TripRouteCodecPoint>[];
  var t = p.t0;
  var lat = 0;
  var lng = 0;
  var spd = 0;
  for (var i = 0; i < p.n; i++) {
    t += p.dt[i];
    lat += p.lat[i];
    lng += p.lng[i];
    spd += p.spd[i];
    result.add(
      TripRouteCodecPoint(
        t: t,
        lat: lat / tripRouteLatLngScale,
        lng: lng / tripRouteLatLngScale,
        spd: spd.toDouble(),
        hdg: p.hdg[i],
        acc: p.acc[i].toDouble(),
        dsm: p.dsm[i].toDouble(),
      ),
    );
  }
  return result;
}
