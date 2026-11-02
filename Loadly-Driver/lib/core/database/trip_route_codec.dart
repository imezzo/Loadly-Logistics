import 'app_database.dart';
import 'trip_route_downsample.dart';

/// 1 unit ≈ 1.1 m at the equator. Bump [tripRouteFormatVersion] if this changes.
const tripRouteLatLngScale = 100000;
const tripRouteFormatVersion = 1;

/// One GPS sample before column encoding (times in Unix seconds).
class TripRouteCodecPoint {
  const TripRouteCodecPoint({
    required this.t,
    required this.lat,
    required this.lng,
    required this.spd,
    required this.hdg,
    required this.acc,
    required this.dsm,
  });

  final int t;
  final double lat;
  final double lng;

  /// Speed km/h (encoded as delta integers in [spd] column).
  final double spd;

  /// Heading degrees [0, 360), absolute in route column.
  final int hdg;

  /// Horizontal accuracy in meters (absolute in [acc] column).
  final double acc;

  /// Distance from previous stored point, meters.
  final double dsm;
}

class EncodedTripRoute {
  const EncodedTripRoute({
    required this.n,
    required this.t0,
    required this.dt,
    required this.lat,
    required this.lng,
    required this.spd,
    required this.hdg,
    required this.acc,
    required this.dsm,
  });

  final int n;
  final int t0;
  final List<int> dt;
  final List<int> lat;
  final List<int> lng;
  final List<int> spd;
  final List<int> hdg;
  final List<int> acc;
  final List<int> dsm;

  Map<String, dynamic> toJson() => {
    'n': n,
    't0': t0,
    'dt': dt,
    'lat': lat,
    'lng': lng,
    'spd': spd,
    'hdg': hdg,
    'acc': acc,
    'dsm': dsm,
  };
}

List<TripRouteCodecPoint> tripRoutePointsFromGpsPackets(List<GpsPacket> packets) {
  final sorted = List<GpsPacket>.from(packets)
    ..sort((a, b) => a.recordedAt.compareTo(b.recordedAt));

  final seen = <int>{};
  final points = <TripRouteCodecPoint>[];
  for (final p in sorted) {
    final t = p.recordedAt.toUtc().millisecondsSinceEpoch ~/ 1000;
    if (seen.contains(t)) continue;
    seen.add(t);

    var hdg = (p.headingDeg ?? 0).round();
    hdg %= 360;
    if (hdg < 0) hdg += 360;

    points.add(
      TripRouteCodecPoint(
        t: t,
        lat: p.latitude,
        lng: p.longitude,
        spd: _mpsToKmh(p.speedMps ?? 0),
        hdg: hdg,
        acc: p.accuracyM ?? 0,
        dsm: p.distanceFromPreviousM ?? 0,
      ),
    );
  }
  return points;
}

/// Column encoding: delta [dt, lat, lng, spd]; absolute [hdg, acc, dsm].
/// [spd] is km/h; [acc] and [dsm] are meters ([dsm] = segment from previous point).
EncodedTripRoute encodeTripRoute(List<TripRouteCodecPoint> points) {
  if (points.isEmpty) {
    throw ArgumentError('No points to encode.');
  }

  final pts = List<TripRouteCodecPoint>.from(points)
    ..sort((a, b) => a.t.compareTo(b.t));

  final n = pts.length;
  final dt = List<int>.filled(n, 0);
  final lat = List<int>.filled(n, 0);
  final lng = List<int>.filled(n, 0);
  final spd = List<int>.filled(n, 0);
  final hdg = List<int>.filled(n, 0);
  final acc = List<int>.filled(n, 0);
  final dsm = List<int>.filled(n, 0);

  var prevT = pts[0].t;
  var prevLat = 0;
  var prevLng = 0;
  var prevSpd = 0;

  for (var i = 0; i < n; i++) {
    final latQ = (pts[i].lat * tripRouteLatLngScale).round();
    final lngQ = (pts[i].lng * tripRouteLatLngScale).round();
    final spdQ = pts[i].spd.round();
    final accQ = pts[i].acc.round();
    final dsmQ = pts[i].dsm.round();

    dt[i] = pts[i].t - prevT;
    lat[i] = latQ - prevLat;
    lng[i] = lngQ - prevLng;
    spd[i] = spdQ - prevSpd;
    hdg[i] = pts[i].hdg;
    acc[i] = accQ;
    dsm[i] = dsmQ;

    prevT = pts[i].t;
    prevLat = latQ;
    prevLng = lngQ;
    prevSpd = spdQ;
  }

  return EncodedTripRoute(
    n: n,
    t0: pts[0].t,
    dt: dt,
    lat: lat,
    lng: lng,
    spd: spd,
    hdg: hdg,
    acc: acc,
    dsm: dsm,
  );
}

/// Encoded upload route plus dense sample count (after per-second dedupe, before downsampling).
class TripRouteUploadBundle {
  const TripRouteUploadBundle({
    required this.route,
    required this.denseGpsPacketCount,
  });

  final EncodedTripRoute route;
  final int denseGpsPacketCount;
}

TripRouteUploadBundle buildTripRouteUploadBundle(
  List<GpsPacket> packets, {
  int maxUploadPoints = tripRouteMaxUploadPoints,
}) {
  final dense = tripRoutePointsFromGpsPackets(packets);
  final upload = prepareTripRoutePointsForUpload(
    dense,
    maxPoints: maxUploadPoints,
  );
  return TripRouteUploadBundle(
    route: encodeTripRoute(upload),
    denseGpsPacketCount: dense.length,
  );
}

EncodedTripRoute encodeTripRouteFromPackets(
  List<GpsPacket> packets, {
  int maxUploadPoints = tripRouteMaxUploadPoints,
}) {
  return buildTripRouteUploadBundle(
    packets,
    maxUploadPoints: maxUploadPoints,
  ).route;
}

double _mpsToKmh(double metersPerSecond) => metersPerSecond * 3.6;
