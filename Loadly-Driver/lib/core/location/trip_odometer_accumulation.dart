import 'package:geolocator/geolocator.dart';

class TripOdometerPoint {
  const TripOdometerPoint(this.latitude, this.longitude, this.timestamp);

  final double latitude;
  final double longitude;
  final DateTime timestamp;
}

class TripOdometerSegmentResult {
  const TripOdometerSegmentResult({
    required this.accepted,
    required this.addedMeters,
    this.nextLast,
  });

  final bool accepted;
  final double addedMeters;
  final TripOdometerPoint? nextLast;
}

const _maxAccuracyMeters = 100.0;
const _minMoveWhenSlowMeters = 15.0;
const _maxSpeedMps = 50.0;
const _maxGapSegmentMeters = 500.0;
const _maxGapSeconds = 180;

TripOdometerSegmentResult accumulateTripOdometerSegment({
  required TripOdometerPoint? last,
  required TripOdometerPoint current,
  required double accuracyMeters,
  double? speedMps,
}) {
  if (accuracyMeters > _maxAccuracyMeters) {
    return const TripOdometerSegmentResult(accepted: false, addedMeters: 0);
  }

  if (last == null) {
    return TripOdometerSegmentResult(
      accepted: true,
      addedMeters: 0,
      nextLast: current,
    );
  }

  final segment = Geolocator.distanceBetween(
    last.latitude,
    last.longitude,
    current.latitude,
    current.longitude,
  );

  final dtSeconds = current.timestamp.difference(last.timestamp).inSeconds;
  if (dtSeconds <= 0 && segment < 1) {
    return const TripOdometerSegmentResult(accepted: false, addedMeters: 0);
  }

  if (speedMps != null && speedMps < 1.5 && segment < _minMoveWhenSlowMeters) {
    return const TripOdometerSegmentResult(accepted: false, addedMeters: 0);
  }

  if (dtSeconds > 0) {
    final impliedSpeed = segment / dtSeconds;
    if (impliedSpeed > _maxSpeedMps) {
      return const TripOdometerSegmentResult(accepted: false, addedMeters: 0);
    }
  }

  var acceptedSegment = segment;
  if (dtSeconds > _maxGapSeconds) {
    acceptedSegment = segment.clamp(0, _maxGapSegmentMeters);
  }

  return TripOdometerSegmentResult(
    accepted: true,
    addedMeters: acceptedSegment,
    nextLast: current,
  );
}
