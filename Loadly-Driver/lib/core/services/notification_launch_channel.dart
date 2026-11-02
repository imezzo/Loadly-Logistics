import 'dart:io';

import 'package:flutter/services.dart';

import '../utils/fcm_notification_logger.dart';

/// Reads Android notification launch intents when Firebase Messaging does not
/// deliver [RemoteMessage] on notification tap.
class NotificationLaunchChannel {
  NotificationLaunchChannel._();

  static const _channel = MethodChannel('com.loadly.driver/notification_launch');

  /// Returns FCM data from the launch intent, or `null` if the app was not
  /// opened from a notification. The map may include `_fromNotificationLaunch`.
  static Future<Map<String, dynamic>?> consumeLaunchNotification() async {
    if (!Platform.isAndroid) return null;

    try {
      final result = await _channel.invokeMethod<Object?>(
        'consumeLaunchNotification',
      );
      if (result is! Map) return null;

      final data = Map<String, dynamic>.from(
        result.map((key, value) => MapEntry(key.toString(), value)),
      );
      if (data.isEmpty) return null;

      FcmNotificationLogger.logStep(
        step: 'android_intent_consumed',
        source: 'notificationLaunchChannel',
        data: data,
      );
      return data;
    } catch (e) {
      FcmNotificationLogger.logStep(
        step: 'android_intent_consume_failed',
        source: 'notificationLaunchChannel',
        detail: e.toString(),
      );
      return null;
    }
  }
}
