import 'package:flutter_test/flutter_test.dart';
import 'package:loadly_driver/core/database/trip_gps_packet_segment.dart';

void main() {
  group('computeGpsPacketSegmentMetrics', () {
    test('first point has zero distance and no heading', () {
      final m = computeGpsPacketSegmentMetrics(
        latitude: 24.0,
        longitude: 46.0,
      );
      expect(m.distanceFromPreviousM, 0);
      expect(m.headingDegrees, isNull);
    });

    test('uses bearing when device heading unavailable', () {
      final m = computeGpsPacketSegmentMetrics(
        latitude: 24.01,
        longitude: 46.0,
        previousLatitude: 24.0,
        previousLongitude: 46.0,
        speedMps: 0.5,
      );
      expect(m.distanceFromPreviousM, greaterThan(1000));
      expect(m.headingDegrees, closeTo(0, 5));
    });

    test('prefers device heading when moving fast enough', () {
      final m = computeGpsPacketSegmentMetrics(
        latitude: 24.01,
        longitude: 46.0,
        previousLatitude: 24.0,
        previousLongitude: 46.0,
        deviceHeadingDegrees: 270,
        speedMps: 10,
      );
      expect(m.headingDegrees, 270);
    });
  });
}
