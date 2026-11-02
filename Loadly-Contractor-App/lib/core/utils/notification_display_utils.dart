import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';

/// Shared Android notification display settings for FCM-backed local notifications.
class NotificationDisplayUtils {
  NotificationDisplayUtils._();

  static DateTime timestampForMessage(RemoteMessage message) {
    return message.sentTime ?? DateTime.now();
  }

  static AndroidNotificationDetails androidDetails({
    required String channelId,
    required String channelName,
    String? channelDescription,
    required DateTime when,
    String? body,
  }) {
    return AndroidNotificationDetails(
      channelId,
      channelName,
      channelDescription: channelDescription,
      icon: '@drawable/ic_notification',
      importance: Importance.high,
      priority: Priority.high,
      onlyAlertOnce: true,
      when: when.millisecondsSinceEpoch,
      showWhen: true,
      styleInformation: body != null && body.isNotEmpty
          ? BigTextStyleInformation(body)
          : null,
    );
  }
}
