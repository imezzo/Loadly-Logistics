import 'package:flutter_test/flutter_test.dart';
import 'package:geolocator/geolocator.dart';
import 'package:loadly_driver/core/location/current_position_reader.dart';

void main() {
  group('GeolocatorCurrentPositionReader', () {
    test('uses the last known position when available', () async {
      final reader = GeolocatorCurrentPositionReader(
        getLastKnownPosition: () async => Position(
          latitude: 12.34,
          longitude: 56.78,
          timestamp: DateTime.utc(2024, 1, 1),
          accuracy: 1.0,
          altitude: 0,
          heading: 0,
          speed: 0,
          speedAccuracy: 0,
          altitudeAccuracy: 0,
          headingAccuracy: 0,
        ),
        getCurrentPosition: (_) async => Position(
          latitude: 99.99,
          longitude: 88.88,
          timestamp: DateTime.utc(2024, 1, 2),
          accuracy: 1.0,
          altitude: 0,
          heading: 0,
          speed: 0,
          speedAccuracy: 0,
          altitudeAccuracy: 0,
          headingAccuracy: 0,
        ),
      );

      final result = await reader.readCurrentPosition();

      expect(result, (12.34, 56.78));
    });

    test('returns null coordinates when location lookup fails', () async {
      final reader = GeolocatorCurrentPositionReader(
        getLastKnownPosition: () async => null,
        getCurrentPosition: (_) async => throw Exception('boom'),
      );

      final result = await reader.readCurrentPosition();

      expect(result, (null, null));
    });
  });
}
