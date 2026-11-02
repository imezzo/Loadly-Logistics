import 'package:shared_preferences/shared_preferences.dart';

import '../utils/fcm_notification_logger.dart';

/// Persists an open-trip deep link when FCM delivers it in the background.
/// Used as a fallback when the OS notification tap does not return data payload.
class NotificationDeepLinkStore {
  NotificationDeepLinkStore._();

  static const _keyRoute = 'notification_pending_trip_route';
  static const _keyMessageId = 'notification_pending_message_id';
  static const _keySavedAt = 'notification_pending_saved_at';
  static const _maxAgeMs = 5 * 60 * 1000;

  static SharedPreferences? _prefs;

  static Future<void> init() async {
    _prefs ??= await SharedPreferences.getInstance();
  }

  static void saveOpenTripRoute({
    required String route,
    String? messageId,
    String source = 'unknown',
  }) {
    final prefs = _prefs;
    if (prefs == null) return;

    prefs.setString(_keyRoute, route);
    prefs.setInt(_keySavedAt, DateTime.now().millisecondsSinceEpoch);
    if (messageId != null) {
      prefs.setString(_keyMessageId, messageId);
    }

    FcmNotificationLogger.logStep(
      step: 'deep_link_saved',
      source: source,
      route: route,
      detail: messageId != null ? 'messageId=$messageId' : null,
    );
  }

  static String? pendingRouteForTap({
    String? messageId,
    String source = 'unknown',
  }) {
    final prefs = _prefs;
    if (prefs == null) return null;

    final savedAt = prefs.getInt(_keySavedAt);
    final route = prefs.getString(_keyRoute);
    if (savedAt == null || route == null) return null;

    final ageMs = DateTime.now().millisecondsSinceEpoch - savedAt;
    if (ageMs > _maxAgeMs) {
      clear(source: '${source}_expired');
      return null;
    }

    final storedMessageId = prefs.getString(_keyMessageId);
    if (messageId != null &&
        storedMessageId != null &&
        messageId != storedMessageId) {
      FcmNotificationLogger.logStep(
        step: 'deep_link_skip',
        source: source,
        route: route,
        detail: 'messageId mismatch tap=$messageId stored=$storedMessageId',
      );
      return null;
    }

    FcmNotificationLogger.logStep(
      step: 'deep_link_restored',
      source: source,
      route: route,
      detail: messageId != null ? 'messageId=$messageId' : null,
    );
    return route;
  }

  static void clear({String source = 'unknown'}) {
    final prefs = _prefs;
    if (prefs == null) return;

    prefs.remove(_keyRoute);
    prefs.remove(_keyMessageId);
    prefs.remove(_keySavedAt);

    FcmNotificationLogger.logStep(
      step: 'deep_link_cleared',
      source: source,
    );
  }
}
