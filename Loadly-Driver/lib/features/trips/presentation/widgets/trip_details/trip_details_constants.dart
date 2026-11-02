import '../../../domain/entities/trip.dart';

/// Trip status codes: 0=Requested, 1=Pending, 2=Scheduled, 3=DriverAccepted, 5=InProgress, 6=Completed, 7=Completed (reviewed), 8=Cancelled
const int kTripRequested = 0;
const int kTripPending = 1;
const int kTripScheduled = 2;
const int kTripDriverAccepted = 3;
const int kTripInProgress = 5;
const int kTripCompleted = kTripStatusCodeCompleted;

/// Waypoint status codes
const int kWaypointPending = 0;
const int kWaypointArrived = 1;
const int kWaypointLoadingStarted = 2;
const int kWaypointLoadingCompleted = 3;
const int kWaypointDeparted = 4;
const int kWaypointCompleted = 5;
const int kWaypointUnloadingStarted = 7;
const int kWaypointUnloadingCompleted = 8;

/// Show checkmark only when waypoint has Departed (4) or Completed (5).
bool isWaypointDepartedOrCompleted(int status) =>
    status == kWaypointDeparted || status == kWaypointCompleted;

/// Returns the next waypoint to navigate to (from driver's current location).
/// - Status 2/3: first waypoint if Pending
/// - Status 5: first Pending waypoint (next stop to arrive at)
TripWaypoint? getNextWaypointToNavigateTo(Trip trip) {
  final waypoints = trip.waypoints ?? [];
  if (waypoints.isEmpty) return null;
  if (trip.statusCode == kTripScheduled ||
      trip.statusCode == kTripDriverAccepted) {
    final first = waypoints.first;
    if (first.status == kWaypointPending &&
        first.latitude != null &&
        first.longitude != null) {
      return first;
    }
    return null;
  }
  if (trip.statusCode == kTripInProgress) {
    for (final wp in waypoints) {
      if (wp.status == kWaypointPending &&
          wp.latitude != null &&
          wp.longitude != null) {
        return wp;
      }
    }
    return null;
  }
  return null;
}

/// True when navigate button should be shown (hide only for Requested/Pending).
bool shouldShowNavigateButton(Trip trip) {
  final code = trip.statusCode;
  return code != kTripRequested && code != kTripPending;
}

/// True when driver has departed from the first waypoint (trip in progress or completed).
bool hasDepartedFromFirstWaypoint(Trip trip) =>
    trip.statusCode == kTripInProgress ||
    tripStatusIsCompleted(trip.statusCode);

/// Returns the origin waypoint for the current leg route (previous waypoint we departed from).
/// Returns null when navigating to the first waypoint (use driver's location as origin).
TripWaypoint? getOriginWaypointForCurrentLeg(Trip trip) {
  final waypoints = trip.waypoints ?? [];
  if (waypoints.isEmpty) return null;
  final next = getNextWaypointToNavigateTo(trip);
  if (next == null) return null;
  final sorted = List<TripWaypoint>.from(waypoints)
    ..sort((a, b) => a.sequence.compareTo(b.sequence));
  final nextIdx = sorted.indexWhere((w) => w.id == next.id);
  if (nextIdx <= 0) return null; // First waypoint - use driver location
  final prev = sorted[nextIdx - 1];
  if (!isWaypointDepartedOrCompleted(prev.status)) return null;
  return prev;
}
