import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';

import '../../features/trips/data/sources/remote/trips_remote_data_source.dart';
import '../../features/trips/domain/entities/trip.dart';
import 'app_database.dart';
import 'trip_route_sync_body.dart';

/// End-of-trip upload of GPS trace + distance.
class TripGpsSyncService {
  TripGpsSyncService(this._database, this._tripsRemote);

  final AppDatabase _database;
  final TripsRemoteDataSource _tripsRemote;

  /// Upload when driver-completed; otherwise discard local trace only.
  Future<void> finalizeEndedTripGpsTrace(
    int tripId, {
    required int? lifecycleActor,
  }) async {
    if (tripGpsRouteSyncAllowed(lifecycleActor)) {
      await syncTripIfPending(tripId);
      return;
    }
    final session = await _database.getSession(tripId);
    if (session == null || session.endedAt == null) return;
    await _database.discardLocalGpsTrace(tripId);
    if (kDebugMode) {
      debugPrint(
        'TripGpsSync discard local trace tripId=$tripId '
        'lifecycleActor=$lifecycleActor',
      );
    }
  }

  /// Upload when trip ended; on success clears packet rows (retention).
  Future<bool> syncTripIfPending(int tripId) async {
    final session = await _database.getSession(tripId);
    if (session == null || session.endedAt == null) return false;
    if (session.syncStatus == 'synced') return true;

    final payload = await _database.buildUploadPayload(tripId);
    if (payload == null) return false;
    if (payload.packets.isEmpty) {
      if (kDebugMode) {
        debugPrint('TripGpsSync skip tripId=$tripId: no GPS packets.');
      }
      return false;
    }

    if (kDebugMode) {
      debugPrint(
        'TripGpsSync pending tripId=$tripId '
        'km=${payload.totalKm.toStringAsFixed(2)} '
        'packets=${payload.packets.length}',
      );
    }

    final uploaded = await _uploadToBackend(payload);
    if (uploaded) {
      await _database.markSessionSynced(tripId);
      return true;
    }
    return false;
  }

  Future<bool> _uploadToBackend(TripGpsUploadPayload payload) async {
    try {
      final body = buildTripRouteSyncBody(payload);
      await _tripsRemote.syncTripRoute(payload.tripId, body);
      return true;
    } on DioException catch (e) {
      if (kDebugMode) {
        debugPrint(
          'TripGpsSync failed tripId=${payload.tripId}: ${e.message}',
        );
      }
      return false;
    } catch (e, st) {
      if (kDebugMode) {
        debugPrint('TripGpsSync failed tripId=${payload.tripId}: $e\n$st');
      }
      return false;
    }
  }
}
