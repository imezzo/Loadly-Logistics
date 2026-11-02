import '../location/trip_odometer_accumulation.dart';
import 'app_database.dart';

/// Sum driven distance from stored GPS packets (ordered by time).
double computeTotalMetersFromGpsPackets(List<GpsPacket> packets) {
  if (packets.isEmpty) return 0;

  final sorted = List<GpsPacket>.from(packets)
    ..sort((a, b) => a.recordedAt.compareTo(b.recordedAt));

  TripOdometerPoint? last;
  var total = 0.0;

  for (final packet in sorted) {
    final current = TripOdometerPoint(
      packet.latitude,
      packet.longitude,
      packet.recordedAt,
    );
    final result = accumulateTripOdometerSegment(
      last: last,
      current: current,
      accuracyMeters: packet.accuracyM ?? 999,
      speedMps: packet.speedMps,
    );
    if (result.accepted) {
      total += result.addedMeters;
      if (result.nextLast != null) {
        last = result.nextLast;
      }
    }
  }

  return total;
}
