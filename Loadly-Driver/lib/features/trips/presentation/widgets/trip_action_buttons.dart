import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../domain/entities/trip.dart';
import '../bloc/trips_bloc.dart';
import '../bloc/trips_event.dart';
import '../bloc/trips_state.dart';

/// Trip status codes from API guide.
const int kScheduled = 2;
const int kDriverAccepted = 3;
const int kFirstPickupLoading = 4;
const int kInProgress = 5;

/// Waypoint status (API enum)
const int kWaypointPending = 0;
const int kWaypointArrived = 1;
const int kWaypointLoadingStarted = 2;
const int kWaypointLoadingCompleted = 3;
const int kWaypointDeparted = 4;
const int kWaypointCompleted = 5;
const int kWaypointUnloadingStarted = 7;
const int kWaypointUnloadingCompleted = 8;

/// Action buttons for trip details based on status and waypoints.
class TripActionButtons extends StatelessWidget {
  const TripActionButtons({
    super.key,
    required this.trip,
    required this.onCallSupport,
  });

  final Trip trip;
  final VoidCallback onCallSupport;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final bloc = context.read<TripsBloc>();
    final isLoading = context.select<TripsBloc, bool>(
      (b) => b.state is TripActionInProgress,
    );

    return Padding(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          // Status 2 (Scheduled): "Start Trip" → POST /trips/{id}/start
          if (trip.statusCode == kScheduled)
            AppButton(
              label: l10n.startTrip,
              loading: isLoading,
              onPressed:
                  _canStartScheduledTrip(
                    startDate: trip.startDateTime,
                    cutoffDuration: trip.cutoffDuration,
                  )
                  ? () => _showStartTripSheet(context, trip, bloc)
                  : null,
            )
          else ...[
            // Start Loading - first pickup when status 3, or later pickups when status 5
            if (_canStartLoading(trip))
              Padding(
                padding: const EdgeInsets.only(top: 12),
                child: AppButton(
                  label: l10n.startLoadingShipment,
                  loading: isLoading,
                  onPressed: () {
                    final wp = _nextPickupToLoad(trip);
                    if (wp == null) return;
                    bloc.add(
                      TripsStartLoading(
                        tripId: trip.id,
                        waypointId: wp.id,
                        latitude: null,
                        longitude: null,
                      ),
                    );
                  },
                ),
              ),

            // Complete Loading - shown in TripStepDocumentsSection
            // (documents collected there, not on-spot)

            // Start Unloading - dropoff waypoint with status Arrived
            if (_canStartUnloading(trip))
              Padding(
                padding: const EdgeInsets.only(top: 12),
                child: AppButton(
                  label: l10n.startUnloading,
                  loading: isLoading,
                  onPressed: () {
                    final wp = _nextDropoffToUnload(trip);
                    if (wp == null) return;
                    bloc.add(
                      TripsStartUnloading(
                        tripId: trip.id,
                        waypointId: wp.id,
                        latitude: null,
                        longitude: null,
                      ),
                    );
                  },
                ),
              ),

            // Complete Unloading - shown in TripStepDocumentsSection

            // Start Trip - status 4, first waypoint LoadingCompleted → POST /trips/{id}/start
            if (_canStartTrip(trip))
              Padding(
                padding: const EdgeInsets.only(top: 12),
                child: AppButton(
                  label: l10n.startTrip,
                  loading: isLoading,
                  onPressed: () => _showStartTripSheet(context, trip, bloc),
                ),
              ),

            // Arrive at waypoint - status 5, next waypoint Pending
            if (_canArrive(trip))
              Padding(
                padding: const EdgeInsets.only(top: 12),
                child: AppButton(
                  label: l10n.arriveAtWaypoint,
                  loading: isLoading,
                  onPressed: () {
                    final wp = _nextWaypointToArrive(trip);
                    if (wp == null) return;
                    bloc.add(
                      TripsArriveAtWaypoint(
                        tripId: trip.id,
                        waypointId: wp.id,
                        latitude: null,
                        longitude: null,
                      ),
                    );
                  },
                ),
              ),

            // Depart - shown in TripStepDocumentsSection

            // Complete Trip - shown in TripStepDocumentsSection
          ],

          CupertinoButton(
            padding: EdgeInsets.zero,
            onPressed: onCallSupport,
            child: Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                const Icon(
                  CupertinoIcons.phone,
                  color: AppColors.mainBlue,
                  size: 20,
                ),
                const SizedBox(width: 8),
                Text(
                  l10n.isThereAProblemCallSupport,
                  style: const TextStyle(
                    color: AppColors.mainBlue,
                    fontSize: 16,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  bool _canStartLoading(Trip t) {
    final waypoints = t.waypoints ?? [];
    if (waypoints.isEmpty) return false;
    // Only show when driver has arrived at the pickup waypoint
    final next = _nextPickupToLoad(t);
    return next != null;
  }

  /// Returns the next pickup where loading can start. Only pickups with status
  /// Arrived are eligible — driver must mark arrival before starting to load.
  TripWaypoint? _nextPickupToLoad(Trip t) {
    final waypoints = t.waypoints ?? [];
    for (final wp in waypoints) {
      if (wp.isPickup && wp.status == kWaypointArrived) return wp;
    }
    return null;
  }

  bool _canStartUnloading(Trip t) {
    return _nextDropoffToUnload(t) != null;
  }

  TripWaypoint? _nextDropoffToUnload(Trip t) {
    final waypoints = t.waypoints ?? [];
    for (final wp in waypoints) {
      if (wp.isDropoff && wp.status == kWaypointArrived) return wp;
    }
    return null;
  }

  bool _canStartTrip(Trip t) {
    if (t.statusCode != kFirstPickupLoading) return false;
    final waypoints = t.waypoints ?? [];
    if (waypoints.isEmpty) return false;
    final first = waypoints.first;
    return first.isPickup && first.status == kWaypointLoadingCompleted;
  }

  bool _canArrive(Trip t) {
    if (t.statusCode != kInProgress && t.statusCode != kDriverAccepted) {
      return false;
    }
    // Arrive only visible when next waypoint has status Pending (0)
    // and previous waypoint has been departed (for subsequent waypoints)
    return _nextWaypointToArrive(t) != null;
  }

  TripWaypoint? _nextWaypointToArrive(Trip t) {
    final waypoints = t.waypoints ?? [];
    for (var i = 0; i < waypoints.length; i++) {
      final wp = waypoints[i];
      if (wp.status != kWaypointPending) continue;
      // First waypoint: no previous, can arrive
      if (i == 0) return wp;
      // Subsequent: only if previous waypoint has departed
      final prev = waypoints[i - 1];
      if (prev.status == kWaypointDeparted ||
          prev.status == kWaypointCompleted) {
        return wp;
      }
    }
    return null;
  }

  /// Scheduled trip can be started if the start time is within 60 minutes from now.
  bool _canStartScheduledTrip({
    required String startDate,
    required int cutoffDuration,
  }) {
    try {
      final difference = DateTime.tryParse(
        startDate,
      )!.toLocal().difference(DateTime.now());
      return difference <= Duration(minutes: trip.cutoffDuration);
    } catch (_) {
      return false;
    }
  }

  void _showStartTripSheet(BuildContext context, Trip trip, TripsBloc bloc) {
    bloc.add(TripsStartTrip(tripId: trip.id, latitude: null, longitude: null));
  }
}
