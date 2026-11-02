import '../../features/trips/domain/entities/trip.dart';
import '../../features/trips/domain/repositories/trips_repository.dart';
import 'trip_odometer_service.dart';

/// Keeps local GPS sessions aligned with server trip status (e.g. admin completion).
class TripGpsSessionReconciler {
  TripGpsSessionReconciler(this._odometer, this._tripsRepository);

  final TripOdometerService _odometer;
  final TripsRepository _tripsRepository;

  /// Applies [trip] status to the odometer (finalize, resume, or no-op).
  Future<void> applyServerTripStatus(Trip trip) async {
    final start = trip.startLocation;
    await _odometer.syncWithTrip(
      tripId: trip.id,
      statusCode: trip.statusCode,
      lifecycleActor: trip.lifecycleActor,
      seedLatitude: start?.latitude,
      seedLongitude: start?.longitude,
    );
  }

  /// Fetches trip details for any open local session and reconciles.
  Future<void> reconcileOpenSessionFromServer() async {
    final tripId = await _odometer.openSessionTripId();
    if (tripId == null) return;

    final result = await _tripsRepository.getTripDetails(tripId);
    await result.fold(
      (_) async => _odometer.resumeTrackingForOpenSession(),
      (trip) async => applyServerTripStatus(trip),
    );
  }

  /// Uses list payloads when possible; otherwise fetches details for the open session.
  Future<void> reconcileFromTrips(List<Trip> trips) async {
    final tripId = await _odometer.openSessionTripId();
    if (tripId == null) return;

    for (final trip in trips) {
      if (trip.id == tripId) {
        await applyServerTripStatus(trip);
        return;
      }
    }

    await reconcileOpenSessionFromServer();
  }
}
