import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';

/// Consistent debug logging for Firebase notification receive, routing, and tap events.
class FcmNotificationLogger {
  FcmNotificationLogger._();

  static const _tag = '[FCM]';

  static void logReceived({
    required String source,
    RemoteMessage? message,
    String? title,
    String? body,
    Map<String, dynamic>? data,
  }) {
    debugPrint(
      '$_tag Notification received | source=$source | '
      '${_formatDetails(message: message, title: title, body: body, data: data)}',
    );
  }

  static void logClicked({
    required String source,
    RemoteMessage? message,
    String? payload,
    String? title,
    String? body,
    Map<String, dynamic>? data,
  }) {
    debugPrint(
      '$_tag Notification clicked | source=$source | '
      '${_formatDetails(message: message, title: title, body: body, data: data, payload: payload)}',
    );
  }

  /// Logs routing / click-action resolution, navigation, and payload handling steps.
  static void logStep({
    required String step,
    required String source,
    RemoteMessage? message,
    Map<String, dynamic>? data,
    String? route,
    String? payload,
    String? detail,
    bool? isLoggedIn,
    bool? routerReady,
  }) {
    final resolvedData = data ?? message?.data ?? const <String, dynamic>{};
    final buffer = StringBuffer()
      ..write('step=$step')
      ..write(' | source=$source');

    if (resolvedData.isNotEmpty) {
      buffer.write(' | data=$resolvedData');
    }
    if (route != null) buffer.write(' | route=$route');
    if (payload != null) buffer.write(' | payload=$payload');
    if (detail != null) buffer.write(' | detail=$detail');
    if (isLoggedIn != null) buffer.write(' | isLoggedIn=$isLoggedIn');
    if (routerReady != null) buffer.write(' | routerReady=$routerReady');

    final messageId = message?.messageId;
    if (messageId != null) {
      buffer.write(' | messageId=$messageId');
    }

    debugPrint('$_tag $buffer');
  }

  static void logError({
    required String step,
    required Object error,
    StackTrace? stackTrace,
  }) {
    debugPrint('$_tag ERROR step=$step | error=$error');
    if (stackTrace != null) {
      debugPrint('$_tag $stackTrace');
    }
  }

  static String _formatDetails({
    RemoteMessage? message,
    String? title,
    String? body,
    Map<String, dynamic>? data,
    String? payload,
  }) {
    final messageId = message?.messageId ?? 'unknown';
    final resolvedTitle =
        title ??
        message?.notification?.title ??
        message?.data['title']?.toString();
    final resolvedBody =
        body ??
        message?.notification?.body ??
        message?.data['body']?.toString() ??
        '';
    final resolvedData = data ?? message?.data ?? const <String, dynamic>{};
    final hasNotificationPayload = message?.notification != null;
    final clickAction = resolvedData['clickAction']?.toString();
    final resourceId = resolvedData['resourceId']?.toString();
    final eventType = resolvedData['eventType']?.toString();
    final resourceType = resolvedData['resourceType']?.toString();

    final buffer = StringBuffer()
      ..write('messageId=$messageId')
      ..write(' | hasNotificationPayload=$hasNotificationPayload')
      ..write(' | title=$resolvedTitle')
      ..write(' | body=$resolvedBody')
      ..write(' | data=$resolvedData');

    if (clickAction != null) buffer.write(' | clickAction=$clickAction');
    if (resourceId != null) buffer.write(' | resourceId=$resourceId');
    if (eventType != null) buffer.write(' | eventType=$eventType');
    if (resourceType != null) buffer.write(' | resourceType=$resourceType');

    if (payload != null && payload.isNotEmpty) {
      buffer.write(' | payload=$payload');
    }

    return buffer.toString();
  }
}
