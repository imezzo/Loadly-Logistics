import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';

import '../utils/fcm_notification_logger.dart';
import '../utils/notification_display_utils.dart';
import 'notification_channels.dart';
import 'notification_payload_parser.dart';

/// Displays FCM messages that the OS will not show itself, and reports taps.
class LocalNotificationService {
  LocalNotificationService({FlutterLocalNotificationsPlugin? plugin})
    : _plugin = plugin ?? FlutterLocalNotificationsPlugin();

  final FlutterLocalNotificationsPlugin _plugin;

  bool _initialized = false;
  void Function(String? payload)? _onTap;

  Future<void> initialize({void Function(String? payload)? onTap}) async {
    if (_initialized) return;
    _onTap = onTap;

    const androidSettings = AndroidInitializationSettings(
      '@drawable/ic_notification',
    );
    const darwinSettings = DarwinInitializationSettings(
      requestAlertPermission: false,
      requestBadgePermission: false,
      requestSoundPermission: false,
    );

    final initialized = await _plugin.initialize(
      settings: const InitializationSettings(
        android: androidSettings,
        iOS: darwinSettings,
        macOS: darwinSettings,
      ),
      onDidReceiveNotificationResponse: _onResponse,
    );

    await _plugin
        .resolvePlatformSpecificImplementation<
          AndroidFlutterLocalNotificationsPlugin
        >()
        ?.createNotificationChannel(
          const AndroidNotificationChannel(
            NotificationChannels.highImportanceId,
            NotificationChannels.highImportanceName,
            description: NotificationChannels.highImportanceDescription,
            importance: Importance.high,
          ),
        );

    _initialized = true;
    FcmNotificationLogger.logStep(
      step: 'local_notifications_initialized',
      source: 'local',
      detail: 'success=$initialized',
    );

    final launch = await _plugin.getNotificationAppLaunchDetails();
    final response = launch?.notificationResponse;
    if ((launch?.didNotificationLaunchApp ?? false) &&
        response?.notificationResponseType ==
            NotificationResponseType.selectedNotification) {
      _deliverTap(response?.payload, source: 'localLaunch');
    }
  }

  Future<void> showRemoteMessage(
    RemoteMessage message, {
    required String source,
  }) async {
    if (!_initialized) {
      FcmNotificationLogger.logStep(
        step: 'local_display_skipped',
        source: source,
        message: message,
        detail: 'plugin not initialized',
      );
      return;
    }

    final title = _text(message, title: true);
    final body = _text(message, title: false);
    if ((title == null || title.isEmpty) && (body == null || body.isEmpty)) {
      FcmNotificationLogger.logStep(
        step: 'local_display_skipped',
        source: source,
        message: message,
        detail: 'missing title and body',
      );
      return;
    }

    final payload = NotificationPayloadParser.encodeLocalPayload(
      message.data,
      messageId: message.messageId,
    );

    try {
      await _plugin.show(
        id: _notificationId(message),
        title: title,
        body: body,
        notificationDetails: NotificationDetails(
          android: NotificationDisplayUtils.androidDetails(
            channelId: NotificationChannels.highImportanceId,
            channelName: NotificationChannels.highImportanceName,
            channelDescription: NotificationChannels.highImportanceDescription,
            when: NotificationDisplayUtils.timestampForMessage(message),
            body: body,
          ),
          iOS: const DarwinNotificationDetails(
            presentAlert: true,
            presentBadge: true,
            presentSound: true,
          ),
        ),
        payload: payload,
      );
      FcmNotificationLogger.logStep(
        step: 'local_display_shown',
        source: source,
        message: message,
        payload: payload,
      );
    } catch (error, stackTrace) {
      FcmNotificationLogger.logError(
        step: 'local_display_failed',
        error: error,
        stackTrace: stackTrace,
      );
    }
  }

  void _onResponse(NotificationResponse response) {
    if (response.notificationResponseType !=
        NotificationResponseType.selectedNotification) {
      return;
    }
    _deliverTap(response.payload, source: 'localTap');
  }

  void _deliverTap(String? payload, {required String source}) {
    FcmNotificationLogger.logClicked(source: source, payload: payload);
    final onTap = _onTap;
    if (onTap == null) return;
    try {
      onTap(payload);
    } catch (error, stackTrace) {
      FcmNotificationLogger.logError(
        step: 'local_tap_failed',
        error: error,
        stackTrace: stackTrace,
      );
    }
  }

  static String? _text(RemoteMessage message, {required bool title}) {
    final notificationText = title
        ? message.notification?.title
        : message.notification?.body;
    final trimmedNotification = notificationText?.trim();
    if (trimmedNotification != null && trimmedNotification.isNotEmpty) {
      return trimmedNotification;
    }

    final dataText = message.data[title ? 'title' : 'body']?.toString().trim();
    if (dataText != null && dataText.isNotEmpty) return dataText;
    return null;
  }

  static int _notificationId(RemoteMessage message) {
    final messageId = message.messageId;
    if (messageId == null || messageId.isEmpty) {
      return DateTime.now().microsecondsSinceEpoch.remainder(1 << 31);
    }
    return messageId.hashCode & 0x7fffffff;
  }
}
