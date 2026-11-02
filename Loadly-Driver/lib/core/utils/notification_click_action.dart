import 'fcm_notification_logger.dart';

/// Resolves navigation targets from FCM / in-app notification click actions.
class NotificationClickAction {
  NotificationClickAction._();

  static const openTrip = 'open_trip';

  static String? _read(Map<String, dynamic> data, String camel, String snake) {
    final value = data[camel] ?? data[snake];
    if (value == null) return null;
    final text = value.toString().trim();
    return text.isEmpty ? null : text;
  }

  static bool _isOpenTrip(Map<String, dynamic> data) {
    final clickAction = _read(data, 'clickAction', 'click_action')?.toLowerCase();
    final resourceType =
        _read(data, 'resourceType', 'resource_type')?.toUpperCase();
    return clickAction == openTrip || resourceType == 'TRIP';
  }

  static String? _resolveTripId(Map<String, dynamic> data) {
    if (!_isOpenTrip(data)) return null;

    return _read(data, 'resourceId', 'resource_id') ??
        _read(data, 'tripId', 'trip_id');
  }

  /// Trip id when notification targets a trip (open_trip or resourceType TRIP).
  static String? tripIdFromData(
    Map<String, dynamic> data, {
    String source = 'unknown',
  }) {
    if (!_isOpenTrip(data)) {
      FcmNotificationLogger.logStep(
        step: 'click_action_skip',
        source: source,
        data: data,
        detail:
            'clickAction=${_read(data, 'clickAction', 'click_action')} '
            'resourceType=${_read(data, 'resourceType', 'resource_type')}',
      );
      return null;
    }

    final tripId = _resolveTripId(data);
    if (tripId == null) {
      FcmNotificationLogger.logStep(
        step: 'click_action_missing_trip_id',
        source: source,
        data: data,
        detail: 'trip target but no resourceId/tripId',
      );
      return null;
    }

    FcmNotificationLogger.logStep(
      step: 'click_action_resolved',
      source: source,
      data: data,
      detail: 'tripId=$tripId',
    );
    return tripId;
  }

  static String? tripRouteFromData(
    Map<String, dynamic> data, {
    String source = 'unknown',
  }) {
    final tripId = tripIdFromData(data, source: source);
    if (tripId == null) return null;

    final route = '/trips/$tripId';
    FcmNotificationLogger.logStep(
      step: 'route_resolved',
      source: source,
      data: data,
      route: route,
    );
    return route;
  }

  static String? localNotificationPayloadFromData(
    Map<String, dynamic> data, {
    String source = 'unknown',
  }) {
    final tripId = _resolveTripId(data);
    if (tripId == null) {
      if (_isOpenTrip(data)) {
        FcmNotificationLogger.logStep(
          step: 'payload_fallback',
          source: source,
          data: data,
          payload: 'generic',
          detail: 'trip target but no trip id',
        );
      }
      return null;
    }

    final payload = 'trip:$tripId';
    FcmNotificationLogger.logStep(
      step: 'payload_resolved',
      source: source,
      data: data,
      payload: payload,
    );
    return payload;
  }

  static String? tripRouteFromNotification({
    String? clickAction,
    required int? resourceId,
    String? resourceType,
    String source = 'unknown',
  }) {
    final normalizedClickAction = clickAction?.trim().toLowerCase();
    final normalizedResourceType = resourceType?.trim().toUpperCase();
    final isOpenTrip =
        normalizedClickAction == openTrip || normalizedResourceType == 'TRIP';

    if (!isOpenTrip || resourceId == null) {
      FcmNotificationLogger.logStep(
        step: 'click_action_skip',
        source: source,
        data: {
          'clickAction': clickAction,
          'resourceId': resourceId,
          'resourceType': resourceType,
        },
        detail: 'in_app_notification',
      );
      return null;
    }

    final route = '/trips/$resourceId';
    FcmNotificationLogger.logStep(
      step: 'route_resolved',
      source: source,
      data: {
        'clickAction': clickAction,
        'resourceId': resourceId,
        'resourceType': resourceType,
      },
      route: route,
      detail: 'in_app_notification',
    );
    return route;
  }
}
