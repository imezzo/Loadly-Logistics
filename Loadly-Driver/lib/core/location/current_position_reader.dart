import 'package:geolocator/geolocator.dart';

abstract interface class CurrentPositionReader {
  Future<(double?, double?)> readCurrentPosition();
}

typedef PositionResolver = Future<Position> Function(LocationSettings settings);
typedef LastKnownPositionResolver = Future<Position?> Function();

class GeolocatorCurrentPositionReader implements CurrentPositionReader {
  const GeolocatorCurrentPositionReader({
    this.settings = const LocationSettings(
      accuracy: LocationAccuracy.low,
      timeLimit: Duration(seconds: 6),
    ),
    this.getCurrentPosition = _defaultGetCurrentPosition,
    this.getLastKnownPosition = _defaultGetLastKnownPosition,
  });

  final LocationSettings settings;
  final PositionResolver getCurrentPosition;
  final LastKnownPositionResolver getLastKnownPosition;

  @override
  Future<(double?, double?)> readCurrentPosition() async {
    try {
      final lastKnownPosition = await getLastKnownPosition();
      if (lastKnownPosition != null) {
        return (lastKnownPosition.latitude, lastKnownPosition.longitude);
      }
    } catch (_) {
      // Fall back to a fresh position lookup when no last known value exists.
    }

    try {
      final position = await getCurrentPosition(settings);
      return (position.latitude, position.longitude);
    } catch (_) {
      return (null, null);
    }
  }

  static Future<Position> _defaultGetCurrentPosition(
    LocationSettings settings,
  ) async {
    return Geolocator.getCurrentPosition(locationSettings: settings);
  }

  static Future<Position?> _defaultGetLastKnownPosition() async {
    return Geolocator.getLastKnownPosition();
  }
}
