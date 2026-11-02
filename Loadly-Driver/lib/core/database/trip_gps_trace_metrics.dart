import 'app_database.dart';
import 'trip_gps_trace_compute.dart';
/// Debug / QA metrics for a completed GPS session.
class TripGpsTraceMetrics {
  const TripGpsTraceMetrics({
    required this.packetCount,
    required this.sessionDuration,
    required this.gapCountOver3Min,
    required this.totalMeters,
  });

  final int packetCount;
  final Duration sessionDuration;
  final int gapCountOver3Min;
  final double totalMeters;

  static TripGpsTraceMetrics fromSession({
    required TripGpsSession session,
    required List<GpsPacket> packets,
  }) {
    final sorted = List<GpsPacket>.from(packets)
      ..sort((a, b) => a.recordedAt.compareTo(b.recordedAt));

    var gaps = 0;
    for (var i = 1; i < sorted.length; i++) {
      final delta = sorted[i].recordedAt.difference(sorted[i - 1].recordedAt);
      if (delta.inSeconds > 180) gaps++;
    }

    final ended = session.endedAt ?? DateTime.now().toUtc();
    return TripGpsTraceMetrics(
      packetCount: sorted.length,
      sessionDuration: ended.difference(session.startedAt),
      gapCountOver3Min: gaps,
      totalMeters: computeTotalMetersFromGpsPackets(sorted),
    );
  }
}
