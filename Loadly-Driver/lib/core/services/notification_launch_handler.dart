import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';

import '../models/trip_schedule_update.dart';
import '../router/app_router.dart';
import '../utils/fcm_notification_logger.dart';
import '../utils/notification_click_action.dart';
import 'local_notification_service.dart';
import 'notification_deep_link_store.dart';
import 'notification_launch_channel.dart';

/// Stores a deep-link route when the app is opened from a notification tap
/// before the router is ready (cold start / splash).
class NotificationLaunchHandler {
  NotificationLaunchHandler._();

  static String? _pendingRoute;
  static RemoteMessage? _pendingMessage;

  static String? get pendingRoute => _pendingRoute;

  @visibleForTesting
  static void resetForTesting() {
    _pendingRoute = null;
    _pendingMessage = null;
    NotificationDeepLinkStore.clear(source: 'resetForTesting');
  }

  /// Persist trip route when FCM delivers open_trip (data may be missing on tap).
  static void cacheFromMessage(
    RemoteMessage message, {
    String source = 'unknown',
  }) {
    final route = NotificationClickAction.tripRouteFromData(
      message.data,
      source: source,
    );
    if (route == null) return;

    NotificationDeepLinkStore.saveOpenTripRoute(
      route: route,
      messageId: message.messageId,
      source: source,
    );
  }

  static void setPendingFromMessage(RemoteMessage message) {
    _pendingMessage = message;
    _pendingRoute = _routeFromMessage(message);
    FcmNotificationLogger.logStep(
      step: 'pending_route_set',
      source: 'setPendingFromMessage',
      message: message,
      route: _pendingRoute,
      detail: 'dataEmpty=${message.data.isEmpty}',
    );
  }

  static void setPendingFromPayload(String payload) {
    _pendingMessage = null;
    FcmNotificationLogger.logStep(
      step: 'pending_payload_received',
      source: 'setPendingFromPayload',
      payload: payload,
    );

    if (payload.startsWith('trip:')) {
      final tripId = payload.replaceFirst('trip:', '').trim();
      if (tripId.isNotEmpty) {
        _pendingRoute = '/trips/$tripId';
        FcmNotificationLogger.logStep(
          step: 'pending_route_set',
          source: 'setPendingFromPayload',
          payload: payload,
          route: _pendingRoute,
          detail: 'open_trip',
        );
        return;
      }
    }
    final update = TripScheduleUpdate.tryParse(payload);
    if (update != null) {
      _pendingRoute = '/home';
      FcmNotificationLogger.logStep(
        step: 'pending_route_set',
        source: 'setPendingFromPayload',
        payload: payload,
        route: _pendingRoute,
        detail: 'trip_schedule_update',
      );
      return;
    }
    if (payload == 'generic') {
      _pendingRoute = '/home';
      FcmNotificationLogger.logStep(
        step: 'pending_route_set',
        source: 'setPendingFromPayload',
        payload: payload,
        route: _pendingRoute,
        detail: 'generic',
      );
    }
  }

  static String? _routeFromMessage(RemoteMessage message) {
    final tripRoute = NotificationClickAction.tripRouteFromData(
      message.data,
      source: 'routeFromMessage',
    );
    if (tripRoute != null) return tripRoute;

    final storedRoute = NotificationDeepLinkStore.pendingRouteForTap(
      messageId: message.messageId,
      source: 'routeFromMessage',
    );
    if (storedRoute != null) return storedRoute;

    FcmNotificationLogger.logStep(
      step: 'route_fallback',
      source: 'routeFromMessage',
      message: message,
      route: '/home',
      detail: 'no trip route in data or store',
    );
    return '/home';
  }

  static void _applyPendingMessageSideEffects() {
    final message = _pendingMessage;
    if (message == null) return;

    final body = message.notification?.body ?? message.data['body'] ?? '';
    if (TripScheduleUpdate.tryParse(body) != null) {
      FcmNotificationLogger.logStep(
        step: 'schedule_update_side_effect',
        source: 'applyPendingMessageSideEffects',
        message: message,
      );
      LocalNotificationService.instance.addTripScheduleUpdateFromMessage(
        message,
      );
    }
  }

  /// Navigate to the pending route once the user is logged in and the router exists.
  static void navigateIfPending({required bool isLoggedIn}) {
    Future.microtask(() => _navigateIfPendingNow(isLoggedIn: isLoggedIn));
  }

  static void _navigateIfPendingNow({required bool isLoggedIn}) {
    final router = AppRouterHolder.instance.router;
    final routerReady = router != null;

    if (!isLoggedIn || _pendingRoute == null) {
      FcmNotificationLogger.logStep(
        step: 'navigate_skipped',
        source: 'navigateIfPending',
        route: _pendingRoute,
        isLoggedIn: isLoggedIn,
        routerReady: routerReady,
        detail: !isLoggedIn
            ? 'user not logged in'
            : _pendingRoute == null
            ? 'no pending route'
            : 'unknown',
      );
      return;
    }

    if (!routerReady) {
      FcmNotificationLogger.logStep(
        step: 'navigate_skipped',
        source: 'navigateIfPending',
        route: _pendingRoute,
        isLoggedIn: isLoggedIn,
        routerReady: false,
        detail: 'router not ready',
      );
      return;
    }

    final route = _pendingRoute!;
    if (_isAlreadyAt(route)) {
      _ignoreBecauseAlreadyOnRoute(route);
      return;
    }
    _pendingRoute = null;
    _applyPendingMessageSideEffects();
    _pendingMessage = null;

    FcmNotificationLogger.logStep(
      step: 'navigate_start',
      source: 'navigateIfPending',
      route: route,
      isLoggedIn: isLoggedIn,
      routerReady: true,
    );
    try {
      router.go(route);
      if (route.startsWith('/trips/')) {
        NotificationDeepLinkStore.clear(source: 'navigateIfPending');
      }
      FcmNotificationLogger.logStep(
        step: 'navigate_success',
        source: 'navigateIfPending',
        route: route,
      );
    } catch (e) {
      FcmNotificationLogger.logStep(
        step: 'navigate_failed',
        source: 'navigateIfPending',
        route: route,
        detail: e.toString(),
      );
    }
  }

  /// Handle Android notification tap when FCM callbacks do not fire.
  static Future<void> handlePlatformNotificationLaunch({
    required bool isLoggedIn,
    void Function()? onRefreshTrips,
  }) async {
    final launchData =
        await NotificationLaunchChannel.consumeLaunchNotification();
    if (launchData == null) return;

    final fromNotificationLaunch =
        launchData.remove('_fromNotificationLaunch') == 'true';
    if (!fromNotificationLaunch) return;

    if (launchData.isNotEmpty) {
      FcmNotificationLogger.logClicked(
        source: 'android_intent_launch',
        data: launchData,
      );
      handleTap(
        RemoteMessage(
          data: launchData.map((key, value) => MapEntry(key, value.toString())),
        ),
        onRefreshTrips: onRefreshTrips,
      );
      return;
    }

    final route = NotificationDeepLinkStore.pendingRouteForTap(
      source: 'android_intent_launch',
    );
    if (route == null) {
      FcmNotificationLogger.logStep(
        step: 'android_intent_no_route',
        source: 'handlePlatformNotificationLaunch',
        detail: 'notification launch but no cached deep link',
      );
      return;
    }

    FcmNotificationLogger.logStep(
      step: 'deep_link_navigate_from_store',
      source: 'handlePlatformNotificationLaunch',
      route: route,
    );
    if (_isAlreadyAt(route)) {
      _ignoreBecauseAlreadyOnRoute(route);
      return;
    }
    _pendingRoute = route;
    onRefreshTrips?.call();
    navigateIfPending(isLoggedIn: isLoggedIn);
  }

  /// Handle an FCM notification tap when the app is already running.
  static void handleTap(
    RemoteMessage message, {
    void Function()? onRefreshTrips,
  }) {
    FcmNotificationLogger.logStep(
      step: 'handle_tap_start',
      source: 'handleTap',
      message: message,
      detail: 'dataEmpty=${message.data.isEmpty}',
    );
    setPendingFromMessage(message);
    final route = _pendingRoute;
    if (route != null && _isAlreadyAt(route)) {
      _ignoreBecauseAlreadyOnRoute(route);
      return;
    }
    if (onRefreshTrips != null) {
      FcmNotificationLogger.logStep(
        step: 'trips_refresh_triggered',
        source: 'handleTap',
        message: message,
      );
      onRefreshTrips();
    }
    navigateIfPending(isLoggedIn: true);
  }

  /// True when [route] is the page already on screen.
  ///
  /// A camera session pauses and resumes the activity. If the original
  /// notification intent is still attached, that resume looks like another
  /// tap and `go` rebuilds the trip page, dropping photos that were just taken.
  static bool _isAlreadyAt(String route) {
    final current = _currentLocation();
    if (current == null || current.isEmpty) return false;
    final normalizedCurrent = _normalizeRoute(current);
    final normalizedRoute = _normalizeRoute(route);
    return normalizedCurrent == normalizedRoute;
  }

  static String _normalizeRoute(String route) {
    if (route.length > 1 && route.endsWith('/')) {
      return route.substring(0, route.length - 1);
    }
    return route;
  }

  static String? _currentLocation() {
    final router = AppRouterHolder.instance.router;
    if (router == null) return null;
    try {
      return router.state.uri.path;
    } catch (_) {
      return null;
    }
  }

  static void _ignoreBecauseAlreadyOnRoute(String route) {
    _applyPendingMessageSideEffects();
    _pendingRoute = null;
    _pendingMessage = null;
    if (route.startsWith('/trips/')) {
      NotificationDeepLinkStore.clear(source: 'already_on_route');
    }
    FcmNotificationLogger.logStep(
      step: 'navigate_skipped',
      source: 'already_on_route',
      route: route,
      detail: 'already showing this route; skip reload',
    );
  }
}
