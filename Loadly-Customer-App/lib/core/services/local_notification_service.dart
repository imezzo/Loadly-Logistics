import 'dart:async';
import 'dart:io';

import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';

import '../models/trip_schedule_update.dart';

/// Channel ID for trip-related notifications
const String _tripScheduleUpdateChannelId = 'trip_schedule_update';
const String _tripScheduleUpdateChannelName = 'Trip Updates';

/// Channel ID for general FCM notifications (foreground)
const String _generalChannelId = 'general_notifications';
const String _generalChannelName = 'Notifications';

/// Handles local notifications for foreground FCM messages and provides
/// a stream of trip schedule updates for in-app display.
class LocalNotificationService {
  LocalNotificationService._();
  static final LocalNotificationService instance = LocalNotificationService._();

  final FlutterLocalNotificationsPlugin _plugin =
      FlutterLocalNotificationsPlugin();

  static const AndroidNotificationDetails _androidDetails =
      AndroidNotificationDetails(
        _tripScheduleUpdateChannelId,
        _tripScheduleUpdateChannelName,
        channelDescription: 'Notifications for trip schedule updates',
        importance: Importance.high,
        priority: Priority.high,
      );

  static const DarwinNotificationDetails _iosDetails =
      DarwinNotificationDetails(
        presentAlert: true,
        presentBadge: true,
        presentSound: true,
      );

  static const AndroidNotificationDetails _generalAndroidDetails =
      AndroidNotificationDetails(
        _generalChannelId,
        _generalChannelName,
        channelDescription: 'General app notifications',
        importance: Importance.high,
        priority: Priority.high,
      );

  bool _initialized = false;
  void Function()? _onScheduleUpdateTap;
  void Function()? _onGenericNotificationTap;

  final _scheduleUpdateController =
      StreamController<TripScheduleUpdate>.broadcast();
  Stream<TripScheduleUpdate> get scheduleUpdateStream =>
      _scheduleUpdateController.stream;

  /// Pending update when app was opened from notification tap (before HomeBloc is ready)
  TripScheduleUpdate? _pendingUpdate;
  TripScheduleUpdate? get pendingUpdate => _pendingUpdate;
  void clearPendingUpdate() {
    _pendingUpdate = null;
  }

  /// Callback when trip schedule update notification is tapped (navigate to home).
  void setOnScheduleUpdateTap(void Function() onTap) {
    _onScheduleUpdateTap = onTap;
  }

  /// Callback when generic notification is tapped (navigate to notifications).
  void setOnGenericNotificationTap(void Function() onTap) {
    _onGenericNotificationTap = onTap;
  }

  /// Initialize local notifications. Call from main() after Firebase.
  Future<void> initialize() async {
    if (_initialized) return;

    const androidInit = AndroidInitializationSettings('@mipmap/ic_launcher');
    const iosInit = DarwinInitializationSettings(
      // FCM already requested but But flutter_local_notifications needs its OWN permission grant to show banners.
      requestAlertPermission: true,
      requestBadgePermission: true,
      requestSoundPermission: true,
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
      final androidPlugin = _plugin
          .resolvePlatformSpecificImplementation<
            AndroidFlutterLocalNotificationsPlugin
          >();
      // Android 13+ requires runtime permission for notifications
      await androidPlugin?.requestNotificationsPermission();
      await androidPlugin?.createNotificationChannel(
        const AndroidNotificationChannel(
          _tripScheduleUpdateChannelId,
          _tripScheduleUpdateChannelName,
          importance: Importance.high,
        ),
      );
      await androidPlugin?.createNotificationChannel(
        const AndroidNotificationChannel(
          _generalChannelId,
          _generalChannelName,
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
      final update = TripScheduleUpdate.tryParse(response.payload);
      if (update != null) {
        _pendingUpdate = update;
        _scheduleUpdateController.add(update);
        _onScheduleUpdateTap?.call();
      } else if (response.payload == 'generic') {
        _onGenericNotificationTap?.call();
      }
    }
  }

  /// Show local notification for trip schedule update (when app is in foreground).
  /// Also adds to stream so home screen can show the card.
  Future<void> showTripScheduleUpdate(RemoteMessage message) async {
    final body = message.notification?.body ?? message.data['body'] ?? '';
    final title =
        message.notification?.title ??
        message.data['title'] ??
        'Trip Schedule Updated';

    final update = TripScheduleUpdate.tryParse(body);
    if (update == null) return;

    // Add to stream for in-app card
    _scheduleUpdateController.add(update);

    // Show local notification (user sees it in foreground)
    await _plugin.show(
      id: message.hashCode % 100000, // Unique ID
      title: title,
      body: update.displayText,
      notificationDetails: const NotificationDetails(
        android: _androidDetails,
        iOS: _iosDetails,
      ),
      payload: body,
    );
  }

  /// Show local notification for any FCM message (when app is in foreground).
  /// Used for messages that are not trip schedule updates.
  Future<void> showForegroundMessage(RemoteMessage message) async {
    final title =
        message.notification?.title ?? message.data['title'] ?? 'Notification';
    final body = message.notification?.body ?? message.data['body'] ?? '';

    debugPrint('Showing local notification: title=$title, body=$body');

    await _plugin.show(
      id:
          message.hashCode % 100000 +
          100000, // Unique ID (offset to avoid trip IDs)
      title: title,
      body: body,
      notificationDetails: const NotificationDetails(
        android: _generalAndroidDetails,
        iOS: _iosDetails,
      ),
      payload: 'generic',
    );
  }

  /// Add trip schedule update to stream (e.g. when FCM notification tapped in background).
  void addTripScheduleUpdateFromMessage(RemoteMessage message) {
    final body = message.notification?.body ?? message.data['body'] ?? '';
    final update = TripScheduleUpdate.tryParse(body);
    if (update != null) {
      _pendingUpdate = update;
      _scheduleUpdateController.add(update);
    }
  }

  void dispose() {
    _scheduleUpdateController.close();
  }
}
