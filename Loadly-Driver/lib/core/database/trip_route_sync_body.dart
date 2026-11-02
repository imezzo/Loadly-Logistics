import 'app_database.dart';
import 'trip_route_codec.dart';

Map<String, dynamic> buildTripRouteSyncBody(TripGpsUploadPayload payload) {
  final endedAt = payload.endedAt;
  if (endedAt == null) {
    throw StateError('Cannot build route sync body without trip end time.');
  }

  final upload = buildTripRouteUploadBundle(payload.packets);

  return {
    'distanceMeters': payload.totalMeters.round(),
    'startTime': payload.startedAt.toUtc().toIso8601String(),
    'endTime': endedAt.toUtc().toIso8601String(),
    'gpsPacketCount': upload.denseGpsPacketCount,
    'route': upload.route.toJson(),
  };
}
