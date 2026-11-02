import 'dart:math' show cos, sin;
import 'dart:ui' show Color;

import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:geolocator/geolocator.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';
import 'package:google_polyline_algorithm/google_polyline_algorithm.dart';

import '../../../../../core/config/google_maps_config.dart';
import '../../../../../core/theme/app_colors.dart';
import '../../../../../core/utils/map_marker_icons.dart';
import '../../../domain/entities/trip.dart';
import 'trip_details_constants.dart';

class _CacheEntry {
  _CacheEntry(this.points, this.expiresAt);
  final List<LatLng> points;
  final DateTime expiresAt;
}

class _RouteStop {
  const _RouteStop(this.point, this.isPickup);
  final LatLng point;
  final bool isPickup;
}

class _ColoredRouteLeg {
  const _ColoredRouteLeg(this.points, this.color);
  final List<LatLng> points;
  final Color color;
}

/// In-memory Directions response cache (short TTL) to reduce duplicate billing.
final Map<String, _CacheEntry> _directionsCache = {};

const Duration _kDirectionsCacheTtl = Duration(minutes: 10);

/// Minimum time between **network** leg Directions calls for the same leg when
/// the driver origin is nearly unchanged (debounce / throttle).
const Duration _kLegMinRefetchInterval = Duration(minutes: 2);

/// With same leg + time window: skip refetch if driver moved less than this (meters).
const double _kLegMinMoveMetersToRefetch = 300;

void _directionsCacheSet(String key, List<LatLng> points) {
  _directionsCache[key] = _CacheEntry(
    points,
    DateTime.now().add(_kDirectionsCacheTtl),
  );
}

List<LatLng>? _directionsCacheGet(String key) {
  final e = _directionsCache[key];
  if (e == null) return null;
  if (DateTime.now().isAfter(e.expiresAt)) {
    _directionsCache.remove(key);
    return null;
  }
  return e.points;
}

double _roundCoord3(double x) => (x * 1000).round() / 1000;

void _logDirectionsNonOk(String context, Map<String, dynamic>? data) {
  if (!kDebugMode || data == null) return;
  final status = data['status'];
  if (status == 'OK') return;
  final msg = data['error_message'];
  debugPrint('Directions API [$context]: status=$status error_message=$msg');
}

/// Handles map updates (routes, markers, polylines) for trip details.
class TripDetailsMapDelegate {
  TripDetailsMapDelegate({
    required void Function(void Function()) setState,
    required bool Function() getMounted,
    void Function(Set<Marker> markers, Set<Polyline> polylines)?
    onMapDataUpdated,
    void Function()? onRefreshRequested,
    this.includeTripStartRoute = false,
  }) : _setState = setState,
       _getMounted = getMounted,
       _onMapDataUpdated = onMapDataUpdated,
       _onRefreshRequested = onRefreshRequested;

  final void Function(void Function()) _setState;
  final bool Function() _getMounted;
  final void Function(Set<Marker> markers, Set<Polyline> polylines)?
  _onMapDataUpdated;
  final void Function()? _onRefreshRequested;

  /// When true (trip success map), draws the road from the trip start through
  /// every stop. Start to the first stop is green. Each later leg is colored
  /// by the pickup or drop-off types at its ends.
  final bool includeTripStartRoute;

  GoogleMapController? controller;
  final Set<Marker> markers = {};
  final Set<Polyline> polylines = {};
  bool hasFittedCamera = false;
  bool isUpdating = false;
  String? _lastTripHash;

  DateTime? _lastLegNetworkAt;
  LatLng? _lastLegNetworkOrigin;
  int? _lastLegNetworkDestWaypointId;
  List<LatLng>? _lastLegNetworkPoints;

  /// Draws the current navigation leg, or the full-trip road overview when
  /// there is no next stop. The overview uses a backend polyline, then Google
  /// Directions, then straight waypoint segments if both are unavailable.
  Future<void> updateMapWithTrip(Trip trip) async {
    if (isUpdating) return;
    final isCompleted = tripStatusIsCompleted(trip.statusCode);
    final dest = getNextWaypointToNavigateTo(trip);
    final mode = (!isCompleted && dest != null) ? 'leg' : 'full';
    final hash = '${_tripHash(trip)}|$mode';
    if (hash == _lastTripHash) {
      retryFitCameraIfNeeded();
      return;
    }
    isUpdating = true;
    try {
      if (!isCompleted && dest != null) {
        await _updateMapWithCurrentLegRoute(trip, dest);
      } else {
        await _updateMapWithTripRoute(trip);
      }
    } finally {
      if (_getMounted()) {
        _lastTripHash = hash;
        isUpdating = false;
      }
    }
  }

  String _tripHash(Trip trip) {
    final wp = trip.waypoints ?? [];
    final statuses = wp.map((w) => '${w.id}:${w.status}').join(',');
    return '${trip.id}|$statuses';
  }

  Future<void> _updateMapWithTripRoute(Trip trip) async {
    final List<LatLng>? routePoints;
    final List<_ColoredRouteLeg> coloredLegs;
    final List<LatLng>? startLegPoints;

    if (includeTripStartRoute) {
      final legsFuture = _resolveColoredWaypointLegs(trip);
      final startLegFuture = _resolveStartToFirstWaypointRoute(trip);
      coloredLegs = await legsFuture;
      startLegPoints = await startLegFuture;
      routePoints = null;
    } else {
      routePoints = await _resolveTripRoute(trip);
      coloredLegs = const [];
      startLegPoints = null;
    }
    if (!_getMounted()) return;

    final tripMarkers = _buildMarkersForTrip(trip);
    final newPolylines = <Polyline>{};

    if (routePoints != null && routePoints.length > 1) {
      newPolylines.add(
        Polyline(
          polylineId: const PolylineId('trip_route'),
          points: routePoints,
          color: AppColors.mainBlue,
          width: 5,
        ),
      );
    }
    for (var i = 0; i < coloredLegs.length; i++) {
      final leg = coloredLegs[i];
      if (leg.points.length < 2) continue;
      newPolylines.add(
        Polyline(
          polylineId: PolylineId('waypoint_leg_$i'),
          points: leg.points,
          color: leg.color,
          width: 5,
        ),
      );
    }
    if (startLegPoints != null && startLegPoints.length > 1) {
      newPolylines.add(
        Polyline(
          polylineId: const PolylineId('start_to_first_waypoint'),
          points: startLegPoints,
          color: AppColors.green,
          width: 5,
          zIndex: 2,
        ),
      );
    }

    _setState(() {
      markers.clear();
      markers.addAll(tripMarkers);
      polylines.clear();
      polylines.addAll(newPolylines);
    });
    _onMapDataUpdated?.call(Set.from(tripMarkers), Set.from(newPolylines));

    final allPoints = <LatLng>[];
    for (final m in tripMarkers) {
      allPoints.add(m.position);
    }
    if (routePoints != null) allPoints.addAll(routePoints);
    for (final leg in coloredLegs) {
      allPoints.addAll(leg.points);
    }
    if (startLegPoints != null) allPoints.addAll(startLegPoints);
    if (allPoints.isNotEmpty &&
        _getMounted() &&
        controller != null &&
        !hasFittedCamera) {
      try {
        _fitCameraToPoints(allPoints);
        hasFittedCamera = true;
      } catch (_) {}
    }
  }

  Future<List<LatLng>?> _resolveTripRoute(Trip trip) async {
    final backendRoute = _routeFromBackendPolyline(trip);
    if (backendRoute != null && backendRoute.length > 1) {
      return backendRoute;
    }

    final cacheKey = 'full|${trip.id}|${_tripHash(trip)}';
    final cached = _directionsCacheGet(cacheKey);
    if (cached != null && cached.length > 1) {
      return cached;
    }

    final directionsRoute = await _routeFromDirectionsApi(trip);
    if (directionsRoute != null && directionsRoute.length > 1) {
      _directionsCacheSet(cacheKey, directionsRoute);
      return directionsRoute;
    }

    return _getWaypointsAsPolyline(trip);
  }

  /// Road route from the recorded trip start to the first stop.
  /// Falls back to a straight segment when Directions is unavailable.
  Future<List<LatLng>?> _resolveStartToFirstWaypointRoute(Trip trip) async {
    final start = trip.startLocation;
    if (start == null) return null;
    final ordered = _buildOrderedRoutePoints(trip);
    if (ordered.isEmpty) return null;

    final origin = LatLng(start.latitude, start.longitude);
    final first = ordered.first;
    const eps = 1e-5;
    if ((origin.latitude - first.latitude).abs() < eps &&
        (origin.longitude - first.longitude).abs() < eps) {
      return null;
    }

    final cacheKey =
        'startleg|${trip.id}|${_roundCoord3(origin.latitude)}|${_roundCoord3(origin.longitude)}|${_roundCoord3(first.latitude)}|${_roundCoord3(first.longitude)}';
    final cached = _directionsCacheGet(cacheKey);
    if (cached != null && cached.length > 1) return cached;

    final routed = await _pointToPointDirections(
      origin: origin,
      destination: first,
      contextLabel: 'start-leg',
    );
    if (routed != null && routed.length > 1) {
      _directionsCacheSet(cacheKey, routed);
      return routed;
    }
    return [origin, first];
  }

  Future<List<LatLng>?> _pointToPointDirections({
    required LatLng origin,
    required LatLng destination,
    required String contextLabel,
  }) async {
    if (kGoogleMapsApiKey.isEmpty) {
      if (kDebugMode) {
        debugPrint(
          'Directions API [$contextLabel]: skipped (empty GOOGLE_MAPS_API_KEY)',
        );
      }
      return null;
    }

    try {
      final dio = Dio();
      final response = await dio.get<Map<String, dynamic>>(
        'https://maps.googleapis.com/maps/api/directions/json',
        queryParameters: {
          'origin': '${origin.latitude},${origin.longitude}',
          'destination': '${destination.latitude},${destination.longitude}',
          'key': kGoogleMapsApiKey,
        },
      );
      final data = response.data;
      _logDirectionsNonOk(contextLabel, data);
      if (data?['status'] != 'OK') return null;
      final routes = data?['routes'] as List?;
      if (routes == null || routes.isEmpty) return null;
      final route = routes.first;
      if (route is! Map) return null;
      final overview = route['overview_polyline'];
      final points = overview is Map ? overview['points']?.toString() : null;
      if (points == null || points.isEmpty) return null;
      final decoded = decodePolyline(points);
      if (decoded.isEmpty) return null;
      return decoded
          .map((c) => LatLng(c[0].toDouble(), c[1].toDouble()))
          .toList();
    } catch (e) {
      if (kDebugMode) {
        debugPrint('Directions API [$contextLabel]: request failed: $e');
      }
      return null;
    }
  }

  List<LatLng>? _routeFromBackendPolyline(Trip trip) {
    final routePolyline = trip.route?.polyline;
    if (routePolyline == null || routePolyline.isEmpty) return null;
    try {
      final decoded = decodePolyline(routePolyline);
      if (decoded.isEmpty) return null;
      return decoded
          .map((c) => LatLng(c[0].toDouble(), c[1].toDouble()))
          .toList();
    } catch (_) {
      return null;
    }
  }

  Future<List<LatLng>?> _routeFromDirectionsApi(Trip trip) async {
    if (kGoogleMapsApiKey.isEmpty) {
      if (kDebugMode) {
        debugPrint(
          'Directions API [full]: skipped (empty GOOGLE_MAPS_API_KEY)',
        );
      }
      return null;
    }

    final orderedPoints = _buildOrderedRoutePoints(trip);
    if (orderedPoints.length < 2) return null;

    const maxWaypoints = 25;
    final origin =
        '${orderedPoints.first.latitude},${orderedPoints.first.longitude}';
    final destination =
        '${orderedPoints.last.latitude},${orderedPoints.last.longitude}';

    String? waypointsParam;
    if (orderedPoints.length > 2) {
      final intermediate = orderedPoints.sublist(1, orderedPoints.length - 1);
      final waypointsForApi = intermediate.length <= maxWaypoints
          ? intermediate
          : _samplePoints(intermediate, maxWaypoints);
      waypointsParam = waypointsForApi
          .map((p) => '${p.latitude},${p.longitude}')
          .join('|');
    }

    try {
      final dio = Dio();
      final response = await dio.get<Map<String, dynamic>>(
        'https://maps.googleapis.com/maps/api/directions/json',
        queryParameters: {
          'origin': origin,
          'destination': destination,
          ...?(waypointsParam != null ? {'waypoints': waypointsParam} : null),
          'key': kGoogleMapsApiKey,
        },
      );
      final data = response.data;
      _logDirectionsNonOk('full', data);
      if (data?['status'] != 'OK') return null;
      final routes = data?['routes'] as List?;
      String? points;
      if (routes != null && routes.isNotEmpty) {
        final route = routes.first;
        if (route is Map<String, dynamic>) {
          final overview = route['overview_polyline'];
          if (overview is Map) {
            points = overview['points']?.toString();
          }
        }
      }
      if (points != null && points.isNotEmpty) {
        final decoded = decodePolyline(points);
        if (decoded.isNotEmpty) {
          return decoded
              .map((c) => LatLng(c[0].toDouble(), c[1].toDouble()))
              .toList();
        }
      }
    } catch (e) {
      if (kDebugMode) debugPrint('Directions API [full]: request failed: $e');
    }
    return null;
  }

  List<LatLng> _buildOrderedRoutePoints(Trip trip) {
    return _buildOrderedRouteStops(trip).map((stop) => stop.point).toList();
  }

  List<_RouteStop> _buildOrderedRouteStops(Trip trip) {
    bool isDuplicate(List<_RouteStop> list, LatLng p) {
      if (list.isEmpty) return false;
      final last = list.last.point;
      const eps = 1e-5;
      return (p.latitude - last.latitude).abs() < eps &&
          (p.longitude - last.longitude).abs() < eps;
    }

    final stops = <_RouteStop>[];
    final waypoints = trip.waypoints ?? [];
    if (waypoints.isNotEmpty) {
      final sorted = List<TripWaypoint>.from(waypoints)
        ..sort((a, b) => a.sequence.compareTo(b.sequence));
      for (final wp in sorted) {
        if (wp.latitude == null || wp.longitude == null) continue;
        final point = LatLng(wp.latitude!, wp.longitude!);
        if (isDuplicate(stops, point)) continue;
        stops.add(_RouteStop(point, wp.isPickup));
      }
    }

    if (stops.isEmpty &&
        trip.pickupLocation.latitude != null &&
        trip.pickupLocation.longitude != null &&
        trip.deliveryLocation.latitude != null &&
        trip.deliveryLocation.longitude != null) {
      stops.add(
        _RouteStop(
          LatLng(
            trip.pickupLocation.latitude!,
            trip.pickupLocation.longitude!,
          ),
          true,
        ),
      );
      stops.add(
        _RouteStop(
          LatLng(
            trip.deliveryLocation.latitude!,
            trip.deliveryLocation.longitude!,
          ),
          false,
        ),
      );
    }

    return stops;
  }

  /// Pickup to pickup is vibrant blue, pickup to drop-off is main blue,
  /// drop-off to pickup is orange, and drop-off to drop-off is red.
  Color _waypointLegColor(bool fromPickup, bool toPickup) {
    if (fromPickup && toPickup) return AppColors.vibrantBlue;
    if (!fromPickup && toPickup) return AppColors.orange;
    if (!fromPickup && !toPickup) return AppColors.red;
    return AppColors.mainBlue;
  }

  Future<List<_ColoredRouteLeg>> _resolveColoredWaypointLegs(Trip trip) async {
    final stops = _buildOrderedRouteStops(trip);
    if (stops.length < 2) return const [];

    final geometries = await _roadGeometriesForStops(
      stops.map((stop) => stop.point).toList(),
    );
    final legs = <_ColoredRouteLeg>[];
    for (var i = 0; i < stops.length - 1; i++) {
      final geometry = i < geometries.length && geometries[i].length > 1
          ? geometries[i]
          : [stops[i].point, stops[i + 1].point];
      legs.add(
        _ColoredRouteLeg(
          geometry,
          _waypointLegColor(stops[i].isPickup, stops[i + 1].isPickup),
        ),
      );
    }
    return legs;
  }

  /// Road geometry for each consecutive stop. One Directions request covers
  /// up to 25 intermediate stops; longer trips are requested in chunks.
  Future<List<List<LatLng>>> _roadGeometriesForStops(List<LatLng> points) async {
    const maxPoints = 27;
    final geometries = <List<LatLng>>[];
    var start = 0;
    while (start < points.length - 1) {
      var end = start + maxPoints - 1;
      if (end > points.length - 1) end = points.length - 1;
      final slice = points.sublist(start, end + 1);
      final routed = await _directionsLegPolylines(slice);
      if (routed != null && routed.length == slice.length - 1) {
        geometries.addAll(routed);
      } else {
        for (var i = 0; i < slice.length - 1; i++) {
          final pair = await _pointToPointDirections(
            origin: slice[i],
            destination: slice[i + 1],
            contextLabel: 'colored-leg',
          );
          geometries.add(
            pair != null && pair.length > 1
                ? pair
                : [slice[i], slice[i + 1]],
          );
        }
      }
      start = end;
    }
    return geometries;
  }

  Future<List<List<LatLng>>?> _directionsLegPolylines(List<LatLng> points) async {
    if (kGoogleMapsApiKey.isEmpty || points.length < 2) return null;

    final origin = '${points.first.latitude},${points.first.longitude}';
    final destination = '${points.last.latitude},${points.last.longitude}';
    String? waypointsParam;
    if (points.length > 2) {
      waypointsParam = points
          .sublist(1, points.length - 1)
          .map((p) => '${p.latitude},${p.longitude}')
          .join('|');
    }

    try {
      final dio = Dio();
      final response = await dio.get<Map<String, dynamic>>(
        'https://maps.googleapis.com/maps/api/directions/json',
        queryParameters: {
          'origin': origin,
          'destination': destination,
          ...?(waypointsParam != null ? {'waypoints': waypointsParam} : null),
          'key': kGoogleMapsApiKey,
        },
      );
      final data = response.data;
      _logDirectionsNonOk('colored-legs', data);
      if (data?['status'] != 'OK') return null;
      final routes = data?['routes'] as List?;
      if (routes == null || routes.isEmpty) return null;
      final route = routes.first;
      if (route is! Map) return null;
      final legs = route['legs'] as List?;
      if (legs == null || legs.length != points.length - 1) return null;

      final result = <List<LatLng>>[];
      for (final leg in legs) {
        if (leg is! Map) return null;
        final decoded = _pointsFromLegSteps(leg);
        if (decoded.length < 2) return null;
        result.add(decoded);
      }
      return result;
    } catch (e) {
      if (kDebugMode) {
        debugPrint('Directions API [colored-legs]: request failed: $e');
      }
      return null;
    }
  }

  List<LatLng> _pointsFromLegSteps(Map<dynamic, dynamic> leg) {
    final steps = leg['steps'] as List?;
    final points = <LatLng>[];
    if (steps == null) return points;
    for (final step in steps) {
      if (step is! Map) continue;
      final poly = step['polyline'];
      final encoded = poly is Map ? poly['points']?.toString() : null;
      if (encoded == null || encoded.isEmpty) continue;
      for (final c in decodePolyline(encoded)) {
        final point = LatLng(c[0].toDouble(), c[1].toDouble());
        if (points.isNotEmpty &&
            points.last.latitude == point.latitude &&
            points.last.longitude == point.longitude) {
          continue;
        }
        points.add(point);
      }
    }
    return points;
  }

  List<LatLng> _samplePoints(List<LatLng> points, int maxCount) {
    if (points.length <= maxCount) return points;
    final result = <LatLng>[];
    final step = (points.length - 1) / (maxCount - 1);
    for (var i = 0; i < maxCount; i++) {
      final idx = (i * step).round().clamp(0, points.length - 1);
      result.add(points[idx]);
    }
    return result;
  }

  List<LatLng>? _getWaypointsAsPolyline(Trip trip) {
    final orderedPoints = _buildOrderedRoutePoints(trip);
    return orderedPoints.length >= 2 ? orderedPoints : null;
  }

  Future<void> _updateMapWithCurrentLegRoute(
    Trip trip,
    TripWaypoint dest,
  ) async {
    final originWaypoint = getOriginWaypointForCurrentLeg(trip);
    final useDriverLocation = originWaypoint == null;

    LatLng? originLatLng;
    if (useDriverLocation) {
      try {
        final userPos = await Geolocator.getCurrentPosition();
        originLatLng = LatLng(userPos.latitude, userPos.longitude);
      } catch (_) {
        if (_getMounted()) updateMapMarkersOnly(trip);
        return;
      }
    } else if (originWaypoint.latitude != null &&
        originWaypoint.longitude != null) {
      originLatLng = LatLng(
        originWaypoint.latitude!,
        originWaypoint.longitude!,
      );
    }
    if (originLatLng == null || !_getMounted()) return;

    final destLat = dest.latitude!;
    final destLng = dest.longitude!;
    final destId = dest.id;

    final oLat = _roundCoord3(originLatLng.latitude);
    final oLng = _roundCoord3(originLatLng.longitude);
    final cacheKey =
        'leg|${trip.id}|$destId|$oLat|$oLng|${_roundCoord3(destLat)}|${_roundCoord3(destLng)}';

    List<LatLng>? routePoints = _directionsCacheGet(cacheKey);

    if (routePoints == null &&
        _shouldThrottleLegNetwork(
          destWaypointId: destId,
          origin: originLatLng,
        )) {
      routePoints = _lastLegNetworkPoints;
    }

    routePoints ??= await _legRouteFromDirectionsApi(
      originLatLng: originLatLng,
      destLat: destLat,
      destLng: destLng,
      cacheKey: cacheKey,
      destWaypointId: destId,
    );

    if (!_getMounted()) return;

    final tripMarkers = _buildMarkersForTrip(trip);
    if (useDriverLocation) {
      tripMarkers.add(
        Marker(
          markerId: const MarkerId('user_location'),
          position: originLatLng,
          icon: BitmapDescriptor.defaultMarkerWithHue(
            BitmapDescriptor.hueAzure,
          ),
          infoWindow: const InfoWindow(title: 'Your location'),
        ),
      );
    }

    final newPolylines = <Polyline>{};
    // Determine the points to draw
    List<LatLng> finalRoutePoints = routePoints ?? [];
    if (finalRoutePoints.length < 2) {
      // Fallback: straight line from origin to destination
      finalRoutePoints = [originLatLng, LatLng(destLat, destLng)];
    }
    // Always draw if we have at least 2 points
    if (finalRoutePoints.length > 1) {
      newPolylines.add(
        Polyline(
          polylineId: const PolylineId('current_leg_route'),
          points: finalRoutePoints,
          color: AppColors.mainBlue,
          width: 5,
        ),
      );
    }

    if (_getMounted()) {
      _setState(() {
        markers.clear();
        markers.addAll(tripMarkers);
        polylines.clear();
        polylines.addAll(newPolylines);
      });
    }

    final allPoints = <LatLng>[originLatLng, LatLng(destLat, destLng)];
    if (_getMounted() && !hasFittedCamera) _fitCameraToPoints(allPoints);
  }

  bool _shouldThrottleLegNetwork({
    required int destWaypointId,
    required LatLng origin,
  }) {
    final lastAt = _lastLegNetworkAt;
    final lastOrigin = _lastLegNetworkOrigin;
    final lastDest = _lastLegNetworkDestWaypointId;
    if (lastAt == null || lastOrigin == null || lastDest == null) {
      return false;
    }
    if (lastDest != destWaypointId) return false;
    final elapsed = DateTime.now().difference(lastAt);
    if (elapsed >= _kLegMinRefetchInterval) return false;
    final moved = Geolocator.distanceBetween(
      lastOrigin.latitude,
      lastOrigin.longitude,
      origin.latitude,
      origin.longitude,
    );
    return moved < _kLegMinMoveMetersToRefetch;
  }

  Future<List<LatLng>?> _legRouteFromDirectionsApi({
    required LatLng originLatLng,
    required double destLat,
    required double destLng,
    required String cacheKey,
    required int destWaypointId,
  }) async {
    if (kGoogleMapsApiKey.isEmpty) {
      if (kDebugMode) {
        debugPrint('Directions API [leg]: skipped (empty GOOGLE_MAPS_API_KEY)');
      }
      return null;
    }

    final origin = '${originLatLng.latitude},${originLatLng.longitude}';
    final destination = '$destLat,$destLng';
    List<LatLng>? routePoints;

    try {
      final dio = Dio();
      if (kDebugMode) {
        debugPrint(
          '🌐 Directions leg request: origin=$origin, dest=$destination',
        );
      }
      final response = await dio.get<Map<String, dynamic>>(
        'https://maps.googleapis.com/maps/api/directions/json',
        queryParameters: {
          'origin': origin,
          'destination': destination,
          'key': kGoogleMapsApiKey,
        },
      );
      final data = response.data;
      if (kDebugMode) {
        debugPrint('📡 Directions leg status: ${data?['status']}');
      }
      _logDirectionsNonOk('leg', data);
      if (data?['status'] != 'OK') return null;
      final routes = data?['routes'] as List?;
      String? points;
      if (routes?.isNotEmpty == true) {
        final overview =
            (routes!.first as Map<String, dynamic>)['overview_polyline'];
        points = overview is Map ? overview['points'] as String? : null;
      }
      if (points != null && points.isNotEmpty) {
        final decoded = decodePolyline(points);
        routePoints = decoded
            .map((c) => LatLng(c[0].toDouble(), c[1].toDouble()))
            .toList();
      }
    } catch (e) {
      if (kDebugMode) debugPrint('Directions API [leg]: request failed: $e');
      routePoints = null;
    }

    if (routePoints != null && routePoints.length > 1) {
      _directionsCacheSet(cacheKey, routePoints);
      _lastLegNetworkAt = DateTime.now();
      _lastLegNetworkOrigin = originLatLng;
      _lastLegNetworkDestWaypointId = destWaypointId;
      _lastLegNetworkPoints = routePoints;
    }

    return routePoints;
  }

  /// Offset in degrees (~15m) for overlapping markers so all are visible.
  static const _overlapOffset = 0.00015;

  /// First pickup stays blue. Later pickups are app orange. Drop-offs are app green.
  BitmapDescriptor _tripStepMarkerIcon({
    required bool isPickup,
    required bool isFirstPickup,
    bool isDepot = false,
  }) {
    if (isDepot) return MapMarkerIcons.dropOff;
    if (!isPickup) return MapMarkerIcons.dropOff;
    return isFirstPickup
        ? MapMarkerIcons.pickupCargo
        : MapMarkerIcons.additionalPickup;
  }

  String _waypointMarkerTitle(TripWaypoint wp) {
    if (wp.isPickup) return 'Pickup';
    if (wp.isDepot) return 'Depot';
    return 'Drop off';
  }

  Set<Marker> _buildMarkersForTrip(Trip trip) {
    final result = <Marker>{};
    const eps = 1e-5;

    void addMarker(
      MarkerId id,
      double lat,
      double lng,
      BitmapDescriptor icon,
      String title,
    ) {
      var useLat = lat;
      var useLng = lng;
      var angleIndex = 0;
      bool overlaps;
      do {
        overlaps = false;
        for (final m in result) {
          if ((m.position.latitude - useLat).abs() < eps &&
              (m.position.longitude - useLng).abs() < eps) {
            overlaps = true;
            angleIndex++;
            final angle = angleIndex * 0.8;
            useLat = lat + _overlapOffset * cos(angle);
            useLng = lng + _overlapOffset * sin(angle);
            break;
          }
        }
      } while (overlaps);
      result.add(
        Marker(
          markerId: id,
          position: LatLng(useLat, useLng),
          icon: icon,
          infoWindow: InfoWindow(title: title),
        ),
      );
    }

    if (includeTripStartRoute && trip.startLocation != null) {
      addMarker(
        const MarkerId('trip_start'),
        trip.startLocation!.latitude,
        trip.startLocation!.longitude,
        BitmapDescriptor.defaultMarkerWithHue(BitmapDescriptor.hueGreen),
        'Trip start',
      );
    }
    if (trip.pickupLocation.latitude != null &&
        trip.pickupLocation.longitude != null) {
      addMarker(
        const MarkerId('start'),
        trip.pickupLocation.latitude!,
        trip.pickupLocation.longitude!,
        MapMarkerIcons.start,
        'Start',
      );
    }
    if (trip.deliveryLocation.latitude != null &&
        trip.deliveryLocation.longitude != null) {
      addMarker(
        const MarkerId('delivery'),
        trip.deliveryLocation.latitude!,
        trip.deliveryLocation.longitude!,
        MapMarkerIcons.dropOff,
        'Drop off',
      );
    }

    final waypoints = trip.waypoints ?? [];
    final sorted = List<TripWaypoint>.from(waypoints)
      ..sort((a, b) => a.sequence.compareTo(b.sequence));
    int? firstPickupId;
    for (final wp in sorted) {
      if (!wp.isPickup) continue;
      firstPickupId = wp.id;
      break;
    }

    if (sorted.isNotEmpty) {
      for (var i = 0; i < sorted.length; i++) {
        final wp = sorted[i];
        if (wp.latitude != null && wp.longitude != null) {
          addMarker(
            MarkerId('wp_$i'),
            wp.latitude!,
            wp.longitude!,
            _tripStepMarkerIcon(
              isPickup: wp.isPickup,
              isFirstPickup: wp.isPickup && wp.id == firstPickupId,
              isDepot: wp.isDepot,
            ),
            _waypointMarkerTitle(wp),
          );
        }
      }
    }

    if (trip.stopPoints != null) {
      for (var i = 0; i < trip.stopPoints!.length; i++) {
        final stop = trip.stopPoints![i];
        if (stop.latitude != null && stop.longitude != null) {
          final isPickup = stop.stopType == 0;
          final isDepot = stop.stopType == 2;
          addMarker(
            MarkerId('stop_$i'),
            stop.latitude!,
            stop.longitude!,
            _tripStepMarkerIcon(
              isPickup: isPickup,
              isFirstPickup: isPickup && stop.id == firstPickupId,
              isDepot: isDepot,
            ),
            isDepot
                ? 'Depot: ${stop.name}'
                : isPickup
                ? 'Pickup: ${stop.name}'
                : 'Drop off: ${stop.name}',
          );
        }
      }
    }

    return result;
  }

  void updateMapMarkersOnly(Trip trip) {
    if (!_getMounted()) return;
    final tripMarkers = _buildMarkersForTrip(trip);

    _setState(() {
      markers.clear();
      markers.addAll(tripMarkers);
      polylines.clear();
    });

    if (tripMarkers.isNotEmpty &&
        _getMounted() &&
        controller != null &&
        !hasFittedCamera) {
      try {
        final points = tripMarkers.map((m) => m.position).toList();
        _fitCameraToPoints(points);
        hasFittedCamera = true;
      } catch (_) {}
    }
  }

  void refreshMapOnResume() {
    if (!_getMounted()) return;
    _onRefreshRequested?.call();
  }

  /// When [updateMapWithTrip] ran before [controller] existed, fit the camera
  /// without another Directions request.
  void retryFitCameraIfNeeded() {
    if (hasFittedCamera || controller == null || !_getMounted()) return;
    final points = <LatLng>[];
    for (final m in markers) {
      points.add(m.position);
    }
    for (final p in polylines) {
      points.addAll(p.points);
    }
    if (points.isEmpty) return;
    try {
      _fitCameraToPoints(points);
      hasFittedCamera = true;
    } catch (_) {}
  }

  void _fitCameraToPoints(List<LatLng> points) {
    if (points.isEmpty || !_getMounted() || controller == null) return;
    double minLat = points.first.latitude;
    double maxLat = points.first.latitude;
    double minLng = points.first.longitude;
    double maxLng = points.first.longitude;
    for (final p in points) {
      if (p.latitude < minLat) minLat = p.latitude;
      if (p.latitude > maxLat) maxLat = p.latitude;
      if (p.longitude < minLng) minLng = p.longitude;
      if (p.longitude > maxLng) maxLng = p.longitude;
    }
    final centerLat = (minLat + maxLat) / 2;
    final centerLng = (minLng + maxLng) / 2;
    final latSpan = maxLat - minLat;
    final lngSpan = maxLng - minLng;
    const minSpan = 0.01;
    if (latSpan < minSpan || lngSpan < minSpan) {
      if (_getMounted() && controller != null) {
        try {
          controller!.animateCamera(
            CameraUpdate.newLatLngZoom(LatLng(centerLat, centerLng), 14),
          );
        } catch (_) {}
      }
      return;
    }
    final bounds = LatLngBounds(
      southwest: LatLng(minLat, minLng),
      northeast: LatLng(maxLat, maxLng),
    );
    if (_getMounted() && controller != null) {
      try {
        controller!.animateCamera(CameraUpdate.newLatLngBounds(bounds, 100));
      } catch (_) {}
    }
  }
}
