import 'dart:async';
import 'dart:io' show Platform;
import 'dart:ui' show Locale;

import 'package:flutter/foundation.dart';
import 'package:geolocator/geolocator.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../../l10n/gen/app_localizations.dart';

typedef PositionListener = void Function(Position position);

/// Single [Geolocator] stream shared by online ping + in-trip GPS trace.
class UnifiedDriverLocationHub {
  UnifiedDriverLocationHub();

  static const _localeKey = 'app_locale';

  StreamSubscription<Position>? _subscription;
  var _tripTraceActive = false;
  var _onlinePingActive = false;
  var _suppressAndroidForegroundNotification = false;
  final _listeners = <PositionListener>{};

  bool get isRunning => _subscription != null;

  void addListener(PositionListener listener) {
    _listeners.add(listener);
    _reconcileStream();
  }

  void removeListener(PositionListener listener) {
    _listeners.remove(listener);
    _reconcileStream();
  }

  void setTripTraceActive(bool active) {
    if (_tripTraceActive == active) return;
    _tripTraceActive = active;
    _reconcileStream();
  }

  void setOnlinePingActive(bool active) {
    if (_onlinePingActive == active) return;
    _onlinePingActive = active;
    _reconcileStream();
  }

  /// Trip distance owns the Android foreground notification while a trip is open.
  void setSuppressAndroidForegroundNotification(bool suppress) {
    if (_suppressAndroidForegroundNotification == suppress) return;
    _suppressAndroidForegroundNotification = suppress;
    if (isRunning) {
      unawaited(_startStream());
    }
  }

  Future<void> dispose() async {
    await _subscription?.cancel();
    _subscription = null;
    _listeners.clear();
  }

  void _reconcileStream() {
    final shouldRun =
        _listeners.isNotEmpty && (_tripTraceActive || _onlinePingActive);
    if (!shouldRun) {
      unawaited(_stopStream());
      return;
    }
    unawaited(_startStream());
  }

  Future<void> _stopStream() async {
    await _subscription?.cancel();
    _subscription = null;
  }

  Future<void> _startStream() async {
    await _subscription?.cancel();

    final permission = await Geolocator.checkPermission();
    if (permission == LocationPermission.denied ||
        permission == LocationPermission.deniedForever) {
      return;
    }
    if (!await Geolocator.isLocationServiceEnabled()) {
      return;
    }

    final onlineStrings = await _onlineNotificationStrings();
    final tripStrings = await _tripNotificationStrings();
    final settings = _tripTraceActive
        ? _tripLocationSettings(
            notificationTitle: tripStrings.title,
            notificationText: tripStrings.body,
          )
        : _onlineLocationSettings(
            notificationTitle: onlineStrings.title,
            notificationText: onlineStrings.body,
          );

    _subscription = Geolocator.getPositionStream(locationSettings: settings)
        .listen(
          _dispatch,
          onError: (Object error) {
            debugPrint('Unified location hub error: $error');
          },
        );
  }

  void _dispatch(Position position) {
    for (final listener in List<PositionListener>.from(_listeners)) {
      listener(position);
    }
  }

  LocationSettings _tripLocationSettings({
    required String notificationTitle,
    required String notificationText,
  }) {
    if (Platform.isAndroid) {
      return AndroidSettings(
        accuracy: LocationAccuracy.high,
        distanceFilter: 15,
        intervalDuration: const Duration(seconds: 10),
        foregroundNotificationConfig: _androidForegroundNotification(
          notificationTitle: notificationTitle,
          notificationText: notificationText,
        ),
      );
    }
    if (Platform.isIOS) {
      return AppleSettings(
        accuracy: LocationAccuracy.high,
        distanceFilter: 15,
        pauseLocationUpdatesAutomatically: false,
        showBackgroundLocationIndicator: true,
        allowBackgroundLocationUpdates: true,
        activityType: ActivityType.automotiveNavigation,
      );
    }
    return const LocationSettings(
      accuracy: LocationAccuracy.high,
      distanceFilter: 15,
    );
  }

  LocationSettings _onlineLocationSettings({
    required String notificationTitle,
    required String notificationText,
  }) {
    if (Platform.isAndroid) {
      return AndroidSettings(
        accuracy: LocationAccuracy.medium,
        distanceFilter: 0,
        intervalDuration: const Duration(minutes: 1),
        foregroundNotificationConfig: _androidForegroundNotification(
          notificationTitle: notificationTitle,
          notificationText: notificationText,
        ),
      );
    }
    if (Platform.isIOS) {
      return AppleSettings(
        accuracy: LocationAccuracy.medium,
        distanceFilter: 0,
        pauseLocationUpdatesAutomatically: false,
        showBackgroundLocationIndicator: true,
        allowBackgroundLocationUpdates: true,
        activityType: ActivityType.automotiveNavigation,
      );
    }
    return const LocationSettings(
      accuracy: LocationAccuracy.medium,
      distanceFilter: 0,
    );
  }

  ForegroundNotificationConfig? _androidForegroundNotification({
    required String notificationTitle,
    required String notificationText,
  }) {
    if (_suppressAndroidForegroundNotification) return null;
    return ForegroundNotificationConfig(
      notificationTitle: notificationTitle,
      notificationText: notificationText,
      notificationChannelName: notificationTitle,
      enableWakeLock: true,
      setOngoing: true,
    );
  }

  Future<({String title, String body})> _onlineNotificationStrings() async {
    final prefs = await SharedPreferences.getInstance();
    final code = prefs.getString(_localeKey) ?? 'ar';
    final l10n = lookupAppLocalizations(Locale(code));
    return (
      title: l10n.locationTrackingNotificationTitle,
      body: l10n.locationTrackingNotificationText,
    );
  }

  Future<({String title, String body})> _tripNotificationStrings() async {
    final prefs = await SharedPreferences.getInstance();
    final code = prefs.getString(_localeKey) ?? 'ar';
    final l10n = lookupAppLocalizations(Locale(code));
    return (
      title: l10n.tripOdometerNotificationTitle,
      body: l10n.tripOdometerNotificationText,
    );
  }
}
