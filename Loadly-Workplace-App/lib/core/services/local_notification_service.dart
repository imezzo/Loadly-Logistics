import 'dart:async';
import 'dart:io';

import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../../l10n/gen/app_localizations.dart';

/// Channel ID for general FCM notifications (foreground)
const String _generalChannelId = 'general_notifications';
const String _localeKey = 'app_locale';

/// Handles local notifications for foreground FCM messages and provides
/// a stream of trip schedule updates for in-app display.
class LocalNotificationService {
  LocalNotificationService._();
  static final LocalNotificationService instance = LocalNotificationService._();

  final FlutterLocalNotificationsPlugin _plugin =
      FlutterLocalNotificationsPlugin();

  static const DarwinNotificationDetails _iosDetails =
      DarwinNotificationDetails(
        presentAlert: true,
        presentBadge: true,
        presentSound: true,
      );

  bool _initialized = false;
  void Function()? _onGenericNotificationTap;

  /// Callback when generic notification is tapped (navigate to notifications).
  void setOnGenericNotificationTap(void Function() onTap) {
    _onGenericNotificationTap = onTap;
  }

  /// Initialize local notifications. Call from main() after Firebase.
  Future<void> initialize() async {
    if (_initialized) return;

    const androidInit = AndroidInitializationSettings('@mipmap/ic_launcher');
    const iosInit = DarwinInitializationSettings(
      requestAlertPermission: false, // FCM already requested
      requestBadgePermission: false,
    );
    const initSettings = InitializationSettings(
      android: androidInit,
      iOS: iosInit,
    );

    await _plugin.initialize(
      settings: initSettings,
      onDidReceiveNotificationResponse: _onNotificationTapped,
    );

    if (Platform.isAndroid) {
      final l10n = await _localizations();
      final androidPlugin = _plugin
          .resolvePlatformSpecificImplementation<
            AndroidFlutterLocalNotificationsPlugin
          >();
      // Android 13+ requires runtime permission for notifications
      await androidPlugin?.requestNotificationsPermission();
      await androidPlugin?.createNotificationChannel(
        AndroidNotificationChannel(
          _generalChannelId,
          l10n.notifications,
          description: l10n.notificationChannelDescription,
          importance: Importance.high,
        ),
      );
    }

    _initialized = true;
    debugPrint('LocalNotificationService initialized');
  }

  void _onNotificationTapped(NotificationResponse response) {
    debugPrint('Local notification tapped: ${response.payload}');
    if (response.payload != null && response.payload!.isNotEmpty) {
        _onGenericNotificationTap?.call();
    }
  }

  /// Show local notification for any FCM message (when app is in foreground).
  Future<void> showForegroundMessage(RemoteMessage message) async {
    final l10n = await _localizations();
    final rawTitle = message.notification?.title ?? message.data['title'];
    final title = rawTitle is String && rawTitle.trim().isNotEmpty
        ? rawTitle
        : l10n.notificationFallbackTitle;
    final body = message.notification?.body ?? message.data['body'] ?? '';

    debugPrint('Showing local notification: title=$title, body=$body');

    await _plugin.show(
      id:
          message.hashCode % 100000 +
          100000, // Unique ID (offset to avoid trip IDs)
      title: title,
      body: body,
      notificationDetails: NotificationDetails(
        android: AndroidNotificationDetails(
          _generalChannelId,
          l10n.notifications,
          channelDescription: l10n.notificationChannelDescription,
          importance: Importance.high,
          priority: Priority.high,
        ),
        iOS: _iosDetails,
      ),
      payload: 'generic',
    );
  }

  Future<AppLocalizations> _localizations() async {
    final prefs = await SharedPreferences.getInstance();
    final code = prefs.getString(_localeKey);
    final locale = (code == null || code.isEmpty)
        ? const Locale('ar')
        : Locale(code);
    return lookupAppLocalizations(locale);
  }
}
