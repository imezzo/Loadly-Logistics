import 'package:flutter_test/flutter_test.dart';
import 'package:geolocator/geolocator.dart';
import 'package:loadly_driver/core/location/trip_odometer_accumulation.dart';

void main() {
  group('accumulateTripOdometerSegment', () {
    test('adds distance between two valid points', () {
      final last = TripOdometerPoint(24.0, 46.0, DateTime(2026, 1, 1, 12));
      final current = TripOdometerPoint(24.001, 46.0, DateTime(2026, 1, 1, 12, 1));
      final segment = Geolocator.distanceBetween(
        last.latitude,
        last.longitude,
        current.latitude,
        current.longitude,
      );

      final result = accumulateTripOdometerSegment(
        last: last,
        current: current,
        accuracyMeters: 10,
        speedMps: 20,
      );

      expect(result.accepted, isTrue);
      expect(result.addedMeters, closeTo(segment, 0.5));
    });

    test('rejects poor accuracy', () {
      final last = TripOdometerPoint(24.0, 46.0, DateTime(2026, 1, 1, 12));
      final current = TripOdometerPoint(24.001, 46.0, DateTime(2026, 1, 1, 12, 1));

      final result = accumulateTripOdometerSegment(
        last: last,
        current: current,
        accuracyMeters: 150,
        speedMps: 20,
      );

      expect(result.accepted, isFalse);
      expect(result.addedMeters, 0);
    });
  });
}
