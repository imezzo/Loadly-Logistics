import 'dart:async';
import 'dart:io' show Platform;

import 'package:flutter/foundation.dart';
import 'package:flutter_foreground_task/flutter_foreground_task.dart';
import 'package:geolocator/geolocator.dart';

import '../../features/trips/domain/entities/trip.dart';
import '../database/app_database.dart';
import '../database/trip_gps_sync_service.dart';
import '../database/trip_gps_trace_compute.dart';
import '../database/trip_gps_trace_metrics.dart';
import 'trip_gps_foreground_task.dart';
import 'trip_gps_ios_relaunch.dart';
import 'trip_gps_trace_writer.dart';
import 'unified_driver_location_hub.dart';

/// GPS trace in Drift + driven distance for UI. End-of-trip API sync is optional.
class TripOdometerService {
  TripOdometerService(
    this._openDatabase,
    this._locationHub,
    this._openSyncService,
  );

  final Future<AppDatabase> Function() _openDatabase;
  final UnifiedDriverLocationHub _locationHub;
  final Future<TripGpsSyncService> Function() _openSyncService;

  final ValueNotifier<TripOdometerUiState> uiState = ValueNotifier(
    const TripOdometerUiState(),
  );

  AppDatabase? _db;
  int? _activeTripId;
  final _writer = TripGpsTraceWriter();
  bool _initialized = false;
  var _taskCallbackRegistered = false;
  PositionListener? _hubListener;

  Future<AppDatabase> get _database async => _db ??= await _openDatabase();

  Future<void> init() async {
    if (_initialized) return;
    _ensureTaskCallback();
    final db = await _database;
    final active = await db.getActiveSession();
    if (active != null) {
      _activeTripId = active.tripId;
      final packets = await db.packetsForTrip(active.tripId);
      _writer.seedFromPackets(packets);
    }
    _initialized = true;
    _emitUi();
  }

  Future<double?> distanceKmForTrip(int tripId) async {
    await init();
    if (_activeTripId == tripId) {
      return _writer.totalMeters / 1000.0;
    }
    final session = await (await _database).getSession(tripId);
    if (session == null) return null;
    if (session.endedAt != null) {
      return session.totalMeters / 1000.0;
    }
    return null;
  }

  Future<double?> finalizedDistanceKm(int tripId) async {
    final session = await (await _database).getSession(tripId);
    if (session == null || session.endedAt == null) return null;
    return session.totalMeters / 1000.0;
  }

  bool isTrackingTrip(int tripId) => _activeTripId == tripId;

  /// Open Drift session trip id, if any.
  Future<int?> openSessionTripId() async {
    await init();
    final db = await _database;
    if (_activeTripId != null) {
      final session = await db.getSession(_activeTripId!);
      if (session != null && session.endedAt == null) {
        return _activeTripId;
      }
    }
    final active = await db.getActiveSession();
    if (active == null) return null;
    _activeTripId = active.tripId;
    return active.tripId;
  }

  /// Reloads driven distance from packets written while the UI was away.
  Future<void> reloadActiveDistanceFromDatabase() async {
    await init();
    if (_activeTripId == null) return;
    final packets = await (await _database).packetsForTrip(_activeTripId!);
    _writer.seedFromPackets(packets);
    _emitUi();
  }

  /// Keeps an open session tracing across background, process death, and relaunch.
  Future<void> resumeTrackingForOpenSession({
    bool promptBatteryExemption = false,
  }) async {
    await init();
    if (_activeTripId == null) return;
    final session = await (await _database).getSession(_activeTripId!);
    if (session == null || session.endedAt != null) return;
    await reloadActiveDistanceFromDatabase();
    await _recordIosRelaunchFix();
    await _startPlatformTracking(
      promptBatteryExemption: promptBatteryExemption,
    );
  }

  Future<void> startTrip(
    int tripId, {
    double? seedLatitude,
    double? seedLongitude,
  }) async {
    await init();
    final db = await _database;

    if (_activeTripId != null && _activeTripId != tripId) {
      await finalizeTrip(_activeTripId!, persistOnly: true);
    }

    _activeTripId = tripId;
    _writer.reset();

    await db.beginTripSession(tripId);

    if (seedLatitude != null && seedLongitude != null) {
      await _recordPacket(
        tripId: tripId,
        latitude: seedLatitude,
        longitude: seedLongitude,
        timestamp: DateTime.now(),
        accuracyMeters: 10,
        speedMps: 0,
        forcePersist: true,
      );
    }

    await db.updateSessionTotalMeters(tripId, _writer.totalMeters);
    await _startPlatformTracking(promptBatteryExemption: true);
    _emitUi();
  }

  /// Resumes open Drift session only — does not start trace before Start trip API.
  Future<void> syncWithTrip({
    required int tripId,
    required int? statusCode,
    int? lifecycleActor,
    double? seedLatitude,
    double? seedLongitude,
  }) async {
    await init();
    final db = await _database;

    if (statusCode == kTripStatusCodeInProgress) {
      final session = await db.getSession(tripId);
      if (session != null && session.endedAt == null) {
        _activeTripId = tripId;
        final packets = await db.packetsForTrip(tripId);
        _writer.seedFromPackets(packets);
        await _startPlatformTracking();
        _emitUi();
      }
      return;
    }

    if (tripStatusIsTerminal(statusCode)) {
      if (_activeTripId == tripId) {
        await finalizeTrip(tripId, lifecycleActor: lifecycleActor);
      } else {
        final session = await db.getSession(tripId);
        if (session != null && session.endedAt == null) {
          await _finalizeOpenSession(tripId, lifecycleActor: lifecycleActor);
        }
        _emitUi();
      }
    }
  }

  Future<double> finalizeTrip(
    int tripId, {
    bool persistOnly = false,
    double? finalLatitude,
    double? finalLongitude,
    int? lifecycleActor,
  }) async {
    await init();

    final wasActive = _activeTripId == tripId;
    if (wasActive) {
      await _stopPlatformTracking();
    }

    if (!persistOnly &&
        wasActive &&
        finalLatitude != null &&
        finalLongitude != null) {
      await _recordPacket(
        tripId: tripId,
        latitude: finalLatitude,
        longitude: finalLongitude,
        timestamp: DateTime.now(),
        accuracyMeters: 25,
        forcePersist: true,
      );
    }

    if (wasActive) {
      await _finalizeOpenSession(tripId, lifecycleActor: lifecycleActor);
      _activeTripId = null;
      _writer.reset();
    } else {
      final db = await _database;
      final session = await db.getSession(tripId);
      if (session != null && session.endedAt == null) {
        await _finalizeOpenSession(tripId, lifecycleActor: lifecycleActor);
      }
    }

    _emitUi();
    return (await (await _database).getSession(tripId))?.totalMeters ?? 0;
  }

  Future<void> _finalizeOpenSession(
    int tripId, {
    int? lifecycleActor,
  }) async {
    final db = await _database;
    final session = await db.getSession(tripId);
    if (session == null) return;

    final packets = await db.packetsForTrip(tripId);
    final totalMeters = computeTotalMetersFromGpsPackets(packets);
    if (_activeTripId == tripId) {
      _writer.seedFromPackets(packets);
    }
    await db.endSession(tripId: tripId, totalMeters: totalMeters);

    if (kDebugMode) {
      final updated = await db.getSession(tripId);
      if (updated != null) {
        final metrics = TripGpsTraceMetrics.fromSession(
          session: updated,
          packets: packets,
        );
        debugPrint(
          'Trip GPS trace tripId=$tripId '
          'packets=${metrics.packetCount} '
          'duration=${metrics.sessionDuration.inMinutes}min '
          'gaps>3m=${metrics.gapCountOver3Min} '
          'km=${(metrics.totalMeters / 1000).toStringAsFixed(2)}',
        );
      }
    }

    final sync = await _openSyncService();
    final traceFinalize = sync.finalizeEndedTripGpsTrace(
      tripId,
      lifecycleActor: lifecycleActor,
    );
    if (tripGpsRouteSyncAllowed(lifecycleActor)) {
      unawaited(traceFinalize);
    } else {
      await traceFinalize;
    }
  }

  Future<void> dispose() async {
    if (_taskCallbackRegistered) {
      FlutterForegroundTask.removeTaskDataCallback(_onForegroundTaskData);
      _taskCallbackRegistered = false;
    }
    _detachHubListener();
    uiState.dispose();
  }

  Future<bool> debugSimulateDrivenMeters(
    int tripId, {
    double meters = 1000,
  }) async {
    if (!kDebugMode) return false;
    await init();
    if (_activeTripId != tripId) {
      debugPrint(
        'Trip odometer debug simulate: trip $tripId is not active ($_activeTripId).',
      );
      return false;
    }

    const stepMeters = 50.0;
    const secondsPerStep = 20;
    const metersPerDegreeLatitude = 111_320.0;

    var latitude = _writer.lastAccepted?.latitude ?? 24.7136;
    var longitude = _writer.lastAccepted?.longitude ?? 46.6753;
    var timestamp = _writer.lastAccepted?.timestamp ?? DateTime.now();

    if (_writer.lastAccepted == null) {
      await _recordPacket(
        tripId: tripId,
        latitude: latitude,
        longitude: longitude,
        timestamp: timestamp,
        accuracyMeters: 10,
        speedMps: 0,
        forcePersist: true,
        source: 'debug_simulate',
      );
    }

    var remaining = meters;
    while (remaining > 0) {
      final step = remaining > stepMeters ? stepMeters : remaining;
      latitude += step / metersPerDegreeLatitude;
      timestamp = timestamp.add(const Duration(seconds: secondsPerStep));
      await _recordPacket(
        tripId: tripId,
        latitude: latitude,
        longitude: longitude,
        timestamp: timestamp,
        accuracyMeters: 10,
        speedMps: step / secondsPerStep,
        forcePersist: true,
        source: 'debug_simulate',
      );
      remaining -= step;
    }

    final db = await _database;
    await db.updateSessionTotalMeters(tripId, _writer.totalMeters);
    _emitUi();
    if (Platform.isAndroid) {
      await TripGpsForegroundController.restart();
    }
    return true;
  }

  Future<void> _recordIosRelaunchFix() async {
    if (!Platform.isIOS || _activeTripId == null) return;
    final fix = await TripGpsIosRelaunch.takePendingFix();
    if (fix == null) return;
    final stored = await _recordPacket(
      tripId: _activeTripId!,
      latitude: fix.latitude,
      longitude: fix.longitude,
      timestamp: fix.timestamp,
      accuracyMeters: fix.accuracyMeters,
      speedMps: fix.speedMps,
      source: 'background',
    );
    if (stored) _emitUi();
  }

  void _ensureTaskCallback() {
    if (!Platform.isAndroid || _taskCallbackRegistered) return;
    FlutterForegroundTask.addTaskDataCallback(_onForegroundTaskData);
    _taskCallbackRegistered = true;
  }

  void _onForegroundTaskData(Object data) {
    if (data is! Map) return;
    if (data['type'] != 'distance') return;
    final tripId = data['tripId'];
    final meters = data['meters'];
    if (tripId is! int || meters is! num) return;
    if (_activeTripId != tripId) return;
    _writer.totalMeters = meters.toDouble();
    _emitUi();
  }

  Future<void> _startPlatformTracking({
    bool promptBatteryExemption = false,
  }) async {
    if (Platform.isAndroid) {
      _ensureTaskCallback();
      _locationHub.setSuppressAndroidForegroundNotification(true);
      await TripGpsForegroundController.ensureRunning(
        promptBatteryExemption: promptBatteryExemption,
      );
      return;
    }
    if (Platform.isIOS) {
      await TripGpsIosRelaunch.arm();
    }
    _attachHubListener();
  }

  Future<void> _stopPlatformTracking() async {
    if (Platform.isAndroid) {
      await TripGpsForegroundController.stop();
      _locationHub.setSuppressAndroidForegroundNotification(false);
      return;
    }
    if (Platform.isIOS) {
      await TripGpsIosRelaunch.disarm();
    }
    _detachHubListener();
  }

  void _attachHubListener() {
    if (_hubListener != null) return;
    _hubListener = (Position position) {
      unawaited(_onPosition(position));
    };
    _locationHub.addListener(_hubListener!);
    _locationHub.setTripTraceActive(true);
  }

  void _detachHubListener() {
    _locationHub.setTripTraceActive(false);
    if (_hubListener != null) {
      _locationHub.removeListener(_hubListener!);
      _hubListener = null;
    }
  }

  Future<void> _onPosition(Position position) async {
    if (_activeTripId == null) return;
    final stored = await _recordPacket(
      tripId: _activeTripId!,
      latitude: position.latitude,
      longitude: position.longitude,
      timestamp: position.timestamp,
      accuracyMeters: position.accuracy,
      speedMps: position.speed >= 0 ? position.speed : null,
      headingDegrees:
          position.heading >= 0 ? position.heading : null,
    );
    if (stored) _emitUi();
  }

  Future<bool> _recordPacket({
    required int tripId,
    required double latitude,
    required double longitude,
    required DateTime timestamp,
    required double accuracyMeters,
    double? speedMps,
    double? headingDegrees,
    bool forcePersist = false,
    String source = 'live',
  }) async {
    final db = await _database;
    final stored = await _writer.record(
      db: db,
      tripId: tripId,
      latitude: latitude,
      longitude: longitude,
      timestamp: timestamp,
      accuracyMeters: accuracyMeters,
      speedMps: speedMps,
      headingDegrees: headingDegrees,
      forcePersist: forcePersist,
      source: source,
    );
    if (!stored) return false;
    await db.updateSessionTotalMeters(tripId, _writer.totalMeters);
    return true;
  }

  void _emitUi() {
    uiState.value = TripOdometerUiState(
      activeTripId: _activeTripId,
      activeMeters: _writer.totalMeters,
      revision: uiState.value.revision + 1,
    );
  }
}

class TripOdometerUiState {
  const TripOdometerUiState({
    this.activeTripId,
    this.activeMeters = 0,
    this.revision = 0,
  });

  final int? activeTripId;
  final double activeMeters;
  final int revision;
}
