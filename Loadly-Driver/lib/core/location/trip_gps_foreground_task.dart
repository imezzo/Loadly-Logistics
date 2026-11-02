import 'dart:async';
import 'dart:io' show Platform;
import 'dart:ui' show DartPluginRegistrant, Locale;

import 'package:flutter/widgets.dart';
import 'package:flutter_foreground_task/flutter_foreground_task.dart';
import 'package:geolocator/geolocator.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:sqlite3_flutter_libs/sqlite3_flutter_libs.dart';

import '../../l10n/gen/app_localizations.dart';
import '../database/app_database.dart';
import 'trip_gps_trace_writer.dart';

const _localeKey = 'app_locale';
const _serviceId = 7101;

/// Android process that keeps writing trip GPS packets after the UI is gone.
class TripGpsForegroundController {
  static var _initialized = false;

  static void ensureInitialized() {
    if (_initialized) return;
    FlutterForegroundTask.init(
      androidNotificationOptions: AndroidNotificationOptions(
        channelId: 'driver_location_access',
        channelName: 'Location',
        channelDescription:
            'Shown while Loadly Driver is accessing your location.',
        onlyAlertOnce: true,
      ),
      iosNotificationOptions: const IOSNotificationOptions(
        showNotification: false,
        playSound: false,
      ),
      foregroundTaskOptions: ForegroundTaskOptions(
        eventAction: ForegroundTaskEventAction.repeat(15000),
        autoRunOnBoot: true,
        autoRunOnMyPackageReplaced: true,
        allowWakeLock: true,
        allowWifiLock: true,
        allowAutoRestart: true,
        stopWithTask: false,
      ),
    );
    _initialized = true;
  }

  static Future<void> ensureRunning({
    bool promptBatteryExemption = false,
  }) async {
    if (!Platform.isAndroid) return;
    ensureInitialized();
    if (promptBatteryExemption) {
      await _prepareAndroidPermissions();
    }
    if (await FlutterForegroundTask.isRunningService) return;

    final copy = await tripGpsNotificationCopy();
    final result = await FlutterForegroundTask.startService(
      serviceId: _serviceId,
      serviceTypes: const [ForegroundServiceTypes.location],
      notificationTitle: copy.title,
      notificationText: copy.body,
      notificationIcon: const NotificationIcon(
        metaDataName: 'com.google.firebase.messaging.default_notification_icon',
      ),
      notificationInitialRoute: '/',
      callback: tripGpsForegroundCallback,
    );
    if (result is ServiceRequestFailure) {
      debugPrint('Trip GPS foreground service failed: ${result.error}');
    }
  }

  static Future<void> stop() async {
    if (!Platform.isAndroid) return;
    if (!await FlutterForegroundTask.isRunningService) return;
    final result = await FlutterForegroundTask.stopService();
    if (result is ServiceRequestFailure) {
      debugPrint('Trip GPS foreground service stop failed: ${result.error}');
    }
  }

  static Future<void> restart() async {
    if (!Platform.isAndroid) return;
    ensureInitialized();
    if (await FlutterForegroundTask.isRunningService) {
      final result = await FlutterForegroundTask.restartService();
      if (result is ServiceRequestFailure) {
        debugPrint(
          'Trip GPS foreground service restart failed: ${result.error}',
        );
      }
      return;
    }
    await ensureRunning();
  }

  static Future<void> _prepareAndroidPermissions() async {
    try {
      final notification =
          await FlutterForegroundTask.checkNotificationPermission();
      if (notification != NotificationPermission.granted) {
        await FlutterForegroundTask.requestNotificationPermission();
      }
    } catch (e) {
      debugPrint('Trip GPS notification permission failed: $e');
    }
    try {
      if (await FlutterForegroundTask.isIgnoringBatteryOptimizations) return;
      final prefs = await SharedPreferences.getInstance();
      const promptedKey = 'trip_gps_battery_prompted';
      if (prefs.getBool(promptedKey) == true) return;
      await FlutterForegroundTask.requestIgnoreBatteryOptimization();
      await prefs.setBool(promptedKey, true);
    } catch (e) {
      debugPrint('Trip GPS battery exemption failed: $e');
    }
  }
}

Future<({String title, String body})> tripGpsNotificationCopy() async {
  final prefs = await SharedPreferences.getInstance();
  final code = prefs.getString(_localeKey) ?? 'ar';
  final l10n = lookupAppLocalizations(Locale(code));
  return (
    title: l10n.tripOdometerNotificationTitle,
    body: l10n.tripOdometerNotificationText,
  );
}

@pragma('vm:entry-point')
void tripGpsForegroundCallback() {
  FlutterForegroundTask.setTaskHandler(TripGpsTaskHandler());
}

class TripGpsTaskHandler extends TaskHandler {
  AppDatabase? _db;
  TripGpsTraceWriter? _writer;
  int? _tripId;
  StreamSubscription<Position>? _subscription;
  Future<void> _tail = Future<void>.value();
  var _streamBroken = false;
  var _starting = false;
  String _title = 'Using your location';
  String _body = 'Loadly Driver is accessing your location.';
  DateTime? _lastNotificationAt;

  @override
  Future<void> onStart(DateTime timestamp, TaskStarter starter) async {
    WidgetsFlutterBinding.ensureInitialized();
    DartPluginRegistrant.ensureInitialized();
    await _beginTrace();
  }

  @override
  void onRepeatEvent(DateTime timestamp) {
    if (_starting) return;
    if (_streamBroken || _subscription == null) {
      unawaited(_beginTrace());
    }
  }

  @override
  Future<void> onDestroy(DateTime timestamp, bool isTimeout) async {
    await _subscription?.cancel();
    _subscription = null;
    try {
      await _tail;
    } catch (e) {
      debugPrint('Trip GPS foreground flush failed: $e');
    }
    await _db?.close();
    _db = null;
  }

  @override
  void onNotificationPressed() {
    FlutterForegroundTask.launchApp('/');
  }

  Future<void> _beginTrace() async {
    if (_starting) return;
    _starting = true;
    try {
      await _subscription?.cancel();
      _subscription = null;
      await applyWorkaroundToOpenSqlite3OnOldAndroidVersions();
      _db ??= await AppDatabase.openDedicated();
      final db = _db!;
      final active = await db.getActiveSession();
      if (active == null) {
        await FlutterForegroundTask.stopService();
        return;
      }

      _tripId = active.tripId;
      final packets = await db.packetsForTrip(active.tripId);
      _writer = TripGpsTraceWriter()..seedFromPackets(packets);
      final copy = await tripGpsNotificationCopy();
      _title = copy.title;
      _body = copy.body;
      _publish(force: true);

      final permission = await Geolocator.checkPermission();
      if (permission == LocationPermission.denied ||
          permission == LocationPermission.deniedForever) {
        _streamBroken = true;
        return;
      }
      if (!await Geolocator.isLocationServiceEnabled()) {
        _streamBroken = true;
        return;
      }

      _streamBroken = false;
      _subscription =
          Geolocator.getPositionStream(
            locationSettings: AndroidSettings(
              accuracy: LocationAccuracy.high,
              distanceFilter: 15,
              intervalDuration: const Duration(seconds: 10),
            ),
          ).listen(
            (position) {
              _tail = _tail.then(
                (_) => _onPosition(position),
                onError: (_, _) {},
              );
            },
            onError: (Object error) {
              debugPrint('Trip GPS foreground stream error: $error');
              _streamBroken = true;
            },
          );
    } catch (e) {
      _streamBroken = true;
      debugPrint('Trip GPS foreground start failed: $e');
    } finally {
      _starting = false;
    }
  }

  Future<void> _onPosition(Position position) async {
    final tripId = _tripId;
    final db = _db;
    final writer = _writer;
    if (tripId == null || db == null || writer == null) return;

    final stored = await writer.record(
      db: db,
      tripId: tripId,
      latitude: position.latitude,
      longitude: position.longitude,
      timestamp: position.timestamp,
      accuracyMeters: position.accuracy,
      speedMps: position.speed >= 0 ? position.speed : null,
      headingDegrees: position.heading >= 0 ? position.heading : null,
      source: 'background',
    );
    if (!stored) return;

    await db.updateSessionTotalMeters(tripId, writer.totalMeters);
    FlutterForegroundTask.sendDataToMain(<String, Object>{
      'type': 'distance',
      'tripId': tripId,
      'meters': writer.totalMeters,
    });
    _publish();
  }

  void _publish({bool force = false}) {
    final now = DateTime.now();
    if (!force &&
        _lastNotificationAt != null &&
        now.difference(_lastNotificationAt!) < const Duration(seconds: 15)) {
      return;
    }
    _lastNotificationAt = now;
    FlutterForegroundTask.updateService(
      notificationTitle: _title,
      notificationText: _body,
    );
  }
}
