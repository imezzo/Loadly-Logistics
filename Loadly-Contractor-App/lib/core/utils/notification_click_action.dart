import '../notifications/notification_intent.dart';
import '../notifications/notification_payload_parser.dart';
import '../notifications/notification_routes.dart';
import 'fcm_notification_logger.dart';

/// Resolves in-app notification taps with the same rules as FCM payloads.
class NotificationClickAction {
  NotificationClickAction._();

  static const openTrip = NotificationClickActions.openTrip;

  static String? tripRouteFromNotification({
    String? clickAction,
    required int? resourceId,
    String? resourceType,
    String source = 'unknown',
  }) {
    final data = <String, dynamic>{
      'clickAction': ?clickAction,
      'resourceId': ?resourceId,
      'resourceType': ?resourceType,
    };
    final intent = NotificationPayloadParser.parse(data);
    if (intent is! OpenTripNotificationIntent) {
      FcmNotificationLogger.logStep(
        step: 'click_action_skip',
        source: source,
        data: data,
        detail: 'in_app_notification',
      );
      return null;
    }

    final route = NotificationRoutes.destinationFor(intent).location;
    FcmNotificationLogger.logStep(
      step: 'route_resolved',
      source: source,
      data: data,
      route: route,
      detail: 'in_app_notification',
    );
    return route;
  }
}
