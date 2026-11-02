import 'package:flutter/widgets.dart';

import '../utils/fcm_notification_logger.dart';
import 'notification_intent.dart';

typedef NotificationDestinationResolver =
    NotificationDestination Function(NotificationIntent intent);

typedef NotificationNavigate =
    bool Function(NotificationDestination destination);

typedef NotificationDrainScheduler = void Function(void Function() action);

/// Holds a notification tap until GoRouter and authentication are both ready.
///
/// The background isolate must not call this. FCM delivers the tap to the
/// main isolate through [getInitialMessage], [onMessageOpenedApp], or a local
/// notification response, and this class performs the navigation there.
class PendingNotificationNavigator {
  PendingNotificationNavigator({
    required this.destinationFor,
    required this.navigate,
    NotificationDrainScheduler? schedule,
  }) : _schedule = schedule ?? _postFrame;

  final NotificationDestinationResolver destinationFor;
  final NotificationNavigate navigate;
  final NotificationDrainScheduler _schedule;

  NotificationIntent? _pending;
  String? _pendingKey;
  final Set<String> _handledKeys = <String>{};

  bool _navigationReady = false;
  bool _sessionResolved = false;
  bool? _isAuthenticated;
  bool _drainScheduled = false;

  bool get hasPendingIntent => _pending != null;

  void markNavigationReady() {
    _navigationReady = true;
    FcmNotificationLogger.logStep(
      step: 'navigation_ready',
      source: 'navigator',
      routerReady: true,
      isLoggedIn: _isAuthenticated,
    );
    _scheduleDrain();
  }

  /// [isAuthenticated] follows the same session the router uses.
  /// A logout drops a pending tap. A cold start that is still logged out
  /// keeps the tap until login finishes.
  void setAuthenticated(bool isAuthenticated) {
    final wasAuthenticated = _isAuthenticated;
    _sessionResolved = true;
    _isAuthenticated = isAuthenticated;

    if (wasAuthenticated == true && !isAuthenticated) {
      _pending = null;
      _pendingKey = null;
      FcmNotificationLogger.logStep(
        step: 'navigation_cleared',
        source: 'navigator',
        detail: 'session ended',
        isLoggedIn: false,
      );
      return;
    }

    _scheduleDrain();
  }

  void enqueue(NotificationIntent intent, {String? dedupeKey}) {
    final key = _normalizedKey(dedupeKey);
    if (key != null && (_handledKeys.contains(key) || _pendingKey == key)) {
      FcmNotificationLogger.logStep(
        step: 'navigation_duplicate_ignored',
        source: 'navigator',
        detail: 'key=$key',
      );
      return;
    }

    _pending = intent;
    _pendingKey = key;
    FcmNotificationLogger.logStep(
      step: 'navigation_enqueued',
      source: 'navigator',
      route: destinationFor(intent).location,
      detail: key == null ? null : 'key=$key',
      isLoggedIn: _isAuthenticated,
      routerReady: _navigationReady,
    );
    _scheduleDrain();
  }

  void _scheduleDrain() {
    if (_pending == null) return;
    if (!_navigationReady || !_sessionResolved || _isAuthenticated != true) {
      FcmNotificationLogger.logStep(
        step: 'navigation_deferred',
        source: 'navigator',
        route: destinationFor(_pending!).location,
        detail: _deferralReason(),
        isLoggedIn: _isAuthenticated,
        routerReady: _navigationReady,
      );
      return;
    }
    if (_drainScheduled) return;
    _drainScheduled = true;
    _schedule(() {
      _drainScheduled = false;
      _drain();
    });
  }

  void _drain() {
    final intent = _pending;
    if (intent == null) return;
    if (!_navigationReady || !_sessionResolved || _isAuthenticated != true) {
      return;
    }

    final destination = destinationFor(intent);
    bool navigated = false;
    try {
      navigated = navigate(destination);
    } catch (error, stackTrace) {
      FcmNotificationLogger.logError(
        step: 'navigation_failed',
        error: error,
        stackTrace: stackTrace,
      );
      return;
    }

    if (!navigated) {
      FcmNotificationLogger.logStep(
        step: 'navigation_deferred',
        source: 'navigator',
        route: destination.location,
        detail: 'navigation callback declined',
        routerReady: _navigationReady,
        isLoggedIn: _isAuthenticated,
      );
      return;
    }

    final key = _pendingKey;
    if (key != null) _handledKeys.add(key);
    _pending = null;
    _pendingKey = null;
    FcmNotificationLogger.logStep(
      step: 'navigation_completed',
      source: 'navigator',
      route: destination.location,
      routerReady: true,
      isLoggedIn: true,
    );
  }

  String _deferralReason() {
    if (!_navigationReady) return 'navigation not ready';
    if (!_sessionResolved) return 'auth session not resolved';
    return 'waiting for authentication';
  }

  static String? _normalizedKey(String? key) {
    final trimmed = key?.trim();
    if (trimmed == null || trimmed.isEmpty) return null;
    return trimmed;
  }

  static void _postFrame(void Function() action) {
    WidgetsBinding.instance.addPostFrameCallback((_) => action());
  }
}
