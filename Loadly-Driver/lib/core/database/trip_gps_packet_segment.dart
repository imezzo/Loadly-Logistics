import 'package:geolocator/geolocator.dart';

/// Distance and heading for one stored GPS packet relative to the prior stored point.
class GpsPacketSegmentMetrics {
  const GpsPacketSegmentMetrics({
    required this.distanceFromPreviousM,
    this.headingDegrees,
  });

  final double distanceFromPreviousM;

  /// Degrees clockwise from north [0, 360), when known.
  final double? headingDegrees;
}

/// Minimum speed (m/s) to trust device-reported heading over bearing.
const _minSpeedForDeviceHeadingMps = 1.5;

/// Minimum segment length (m) to compute bearing-based heading.
const _minSegmentForBearingM = 1.0;

GpsPacketSegmentMetrics computeGpsPacketSegmentMetrics({
  required double latitude,
  required double longitude,
  double? previousLatitude,
  double? previousLongitude,
  double? deviceHeadingDegrees,
  double? speedMps,
}) {
  if (previousLatitude == null || previousLongitude == null) {
    return const GpsPacketSegmentMetrics(distanceFromPreviousM: 0);
  }

  final distanceFromPreviousM = Geolocator.distanceBetween(
    previousLatitude,
    previousLongitude,
    latitude,
    longitude,
  );

  double? headingDegrees;
  final deviceHeadingValid =
      deviceHeadingDegrees != null && deviceHeadingDegrees >= 0;
  if (deviceHeadingValid && (speedMps ?? 0) >= _minSpeedForDeviceHeadingMps) {
    headingDegrees = _normalizeHeading(deviceHeadingDegrees);
  } else if (distanceFromPreviousM >= _minSegmentForBearingM) {
    headingDegrees = _normalizeHeading(
      Geolocator.bearingBetween(
        previousLatitude,
        previousLongitude,
        latitude,
        longitude,
      ),
    );
  }

  return GpsPacketSegmentMetrics(
    distanceFromPreviousM: distanceFromPreviousM,
    headingDegrees: headingDegrees,
  );
}

double _normalizeHeading(double degrees) {
  var h = degrees % 360;
  if (h < 0) h += 360;
  return h;
}
