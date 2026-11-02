import 'dart:io';

import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';
import '../utils/fcm_notification_logger.dart';
import '../utils/notification_click_action.dart';
import '../utils/notification_display_utils.dart';
import 'notification_launch_handler.dart';

const _generalChannelId = 'general_notifications';
const _generalChannelName = 'Notifications';

final FlutterLocalNotificationsPlugin _backgroundNotificationsPlugin =
    FlutterLocalNotificationsPlugin();

bool _backgroundNotificationsInitialized = false;

Future<void> _ensureBackgroundNotificationsInitialized() async {
  if (_backgroundNotificationsInitialized) return;

  const androidInit =
      AndroidInitializationSettings('@drawable/ic_notification');
  const initSettings = InitializationSettings(android: androidInit);
  await _backgroundNotificationsPlugin.initialize(settings: initSettings);

  if (Platform.isAndroid) {
    final androidPlugin =
        _backgroundNotificationsPlugin.resolvePlatformSpecificImplementation<
            AndroidFlutterLocalNotificationsPlugin>();
    await androidPlugin?.createNotificationChannel(
      const AndroidNotificationChannel(
        _generalChannelId,
        _generalChannelName,
        importance: Importance.high,
      ),
    );
  }

  _backgroundNotificationsInitialized = true;
}

Future<void> _showDataOnlyNotification(RemoteMessage message) async {
  final title = message.data['title']?.toString() ?? 'Notification';
  final body = message.data['body']?.toString() ?? '';
  FcmNotificationLogger.logReceived(
    source: 'background_local_display',
    message: message,
    title: title,
    body: body,
  );

  final payload = NotificationClickAction.localNotificationPayloadFromData(
        message.data,
        source: 'background_local_display',
      ) ??
      'generic';
  FcmNotificationLogger.logStep(
    step: 'local_notification_show',
    source: 'background_local_display',
    message: message,
    payload: payload,
  );

  await _backgroundNotificationsPlugin.show(
    id: message.hashCode.abs() % 100000 + 200000,
    title: title,
    body: body,
    notificationDetails: NotificationDetails(
      android: NotificationDisplayUtils.androidDetails(
        channelId: _generalChannelId,
        channelName: _generalChannelName,
        when: NotificationDisplayUtils.timestampForMessage(message),
      ),
    ),
    payload: payload,
  );
}

/// Top-level function to handle background messages.
/// Must be registered in main() before runApp().
@pragma('vm:entry-point')
Future<void> firebaseMessagingBackgroundHandler(RemoteMessage message) async {
  await Firebase.initializeApp();

  FcmNotificationLogger.logReceived(
    source: 'background',
    message: message,
  );

  NotificationLaunchHandler.cacheFromMessage(
    message,
    source: 'backgroundHandler',
  );

  // When the payload is data-only, the OS won't show a notification while the
  // app is killed — display one locally so the tap can launch the app.
  if (message.notification == null && message.data.isNotEmpty) {
    FcmNotificationLogger.logStep(
      step: 'background_show_local',
      source: 'backgroundHandler',
      message: message,
      detail: 'data-only message',
    );
    await _ensureBackgroundNotificationsInitialized();
    await _showDataOnlyNotification(message);
  } else {
    FcmNotificationLogger.logStep(
      step: 'background_os_handles',
      source: 'backgroundHandler',
      message: message,
      detail:
          'notification payload present — OS shows notification; deep link cached for tap',
    );
  }
}
