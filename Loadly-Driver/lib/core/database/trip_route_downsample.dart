import 'package:geolocator/geolocator.dart';

import 'trip_route_codec.dart';

/// Max GPS samples in the encoded route payload sent to the backend.
const tripRouteMaxUploadPoints = 30;

/// Downsamples for upload and recomputes [TripRouteCodecPoint.dsm] along the
/// kept sequence (not the original dense trace).
List<TripRouteCodecPoint> prepareTripRoutePointsForUpload(
  List<TripRouteCodecPoint> points, {
  int maxPoints = tripRouteMaxUploadPoints,
}) {
  if (points.isEmpty) return const [];

  final sorted = List<TripRouteCodecPoint>.from(points)
    ..sort((a, b) => a.t.compareTo(b.t));

  if (sorted.length <= maxPoints) {
    return recomputeDsmAlongSelectedPoints(sorted);
  }

  final selectedIndices = _selectIndicesByPathDistance(sorted, maxPoints);
  final selected = selectedIndices.map((i) => sorted[i]).toList();
  return recomputeDsmAlongSelectedPoints(selected);
}

/// Geodesic segment length between consecutive selected points.
List<TripRouteCodecPoint> recomputeDsmAlongSelectedPoints(
  List<TripRouteCodecPoint> selected,
) {
  if (selected.isEmpty) return const [];

  final out = <TripRouteCodecPoint>[];
  for (var i = 0; i < selected.length; i++) {
    final p = selected[i];
    final dsm = i == 0
        ? 0.0
        : Geolocator.distanceBetween(
            selected[i - 1].lat,
            selected[i - 1].lng,
            p.lat,
            p.lng,
          );
    out.add(
      TripRouteCodecPoint(
        t: p.t,
        lat: p.lat,
        lng: p.lng,
        spd: p.spd,
        hdg: p.hdg,
        acc: p.acc,
        dsm: dsm,
      ),
    );
  }
  return out;
}

List<int> _selectIndicesByPathDistance(
  List<TripRouteCodecPoint> sorted,
  int maxPoints,
) {
  final n = sorted.length;
  if (n <= maxPoints) {
    return List.generate(n, (i) => i);
  }

  final cumul = List<double>.filled(n, 0);
  for (var i = 1; i < n; i++) {
    cumul[i] =
        cumul[i - 1] +
        Geolocator.distanceBetween(
          sorted[i - 1].lat,
          sorted[i - 1].lng,
          sorted[i].lat,
          sorted[i].lng,
        );
  }

  final total = cumul[n - 1];
  if (total <= 0) {
    return _selectIndicesUniformTime(n, maxPoints);
  }

  final indices = <int>{0, n - 1};
  final segments = maxPoints - 1;
  for (var k = 1; k < segments; k++) {
    final target = total * k / segments;
    indices.add(_indexAtOrAfterArcLength(cumul, target));
  }

  final list = indices.toList()..sort();
  return list;
}

/// Evenly spaced indices when all fixes share one location.
List<int> _selectIndicesUniformTime(int n, int maxPoints) {
  if (n <= maxPoints) return List.generate(n, (i) => i);
  final indices = <int>{0, n - 1};
  final segments = maxPoints - 1;
  for (var k = 1; k < segments; k++) {
    final idx = ((n - 1) * k / segments).round();
    indices.add(idx.clamp(0, n - 1));
  }
  return indices.toList()..sort();
}

int _indexAtOrAfterArcLength(List<double> cumul, double target) {
  var lo = 0;
  var hi = cumul.length - 1;
  while (lo < hi) {
    final mid = (lo + hi) ~/ 2;
    if (cumul[mid] < target) {
      lo = mid + 1;
    } else {
      hi = mid;
    }
  }
  return lo;
}
