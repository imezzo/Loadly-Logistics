import 'dart:async';
import 'dart:io';

import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';

import '../models/trip_schedule_update.dart';
import '../utils/fcm_notification_logger.dart';
import '../utils/notification_click_action.dart';
import '../utils/notification_display_utils.dart';
import 'notification_launch_handler.dart';

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

  static const DarwinNotificationDetails _iosDetails =
      DarwinNotificationDetails(
    presentAlert: true,
    presentBadge: true,
    presentSound: true,
  );

  NotificationDetails _notificationDetailsFor(
    RemoteMessage message, {
    required String channelId,
    required String channelName,
    String? channelDescription,
    required String body,
  }) {
    return NotificationDetails(
      android: NotificationDisplayUtils.androidDetails(
        channelId: channelId,
        channelName: channelName,
        channelDescription: channelDescription,
        when: NotificationDisplayUtils.timestampForMessage(message),
        body: body,
      ),
      iOS: _iosDetails,
    );
  }

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

    const androidInit =
        AndroidInitializationSettings('@drawable/ic_notification');
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
      final androidPlugin = _plugin.resolvePlatformSpecificImplementation<
          AndroidFlutterLocalNotificationsPlugin>();
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

    final launchDetails = await _plugin.getNotificationAppLaunchDetails();
    if (launchDetails?.didNotificationLaunchApp ?? false) {
      final response = launchDetails!.notificationResponse;
      if (response != null) {
        FcmNotificationLogger.logClicked(
          source: 'local_notification_cold_start',
          payload: response.payload,
        );
        _handleNotificationPayload(response.payload, fromColdStart: true);
      }
    }

    _initialized = true;
    FcmNotificationLogger.logStep(
      step: 'service_initialized',
      source: 'localNotificationService',
    );
  }

  void _onNotificationTapped(NotificationResponse response) {
    FcmNotificationLogger.logClicked(
      source: 'local_notification_tap',
      payload: response.payload,
    );
    _handleNotificationPayload(response.payload);
  }

  @visibleForTesting
  void handlePayloadForTesting(
    String? payload, {
    bool fromColdStart = false,
  }) {
    _handleNotificationPayload(payload, fromColdStart: fromColdStart);
  }

  void _handleNotificationPayload(
    String? payload, {
    bool fromColdStart = false,
  }) {
    FcmNotificationLogger.logStep(
      step: 'payload_handle_start',
      source: 'localNotificationService',
      payload: payload,
      detail: 'fromColdStart=$fromColdStart',
    );

    if (payload == null || payload.isEmpty) {
      FcmNotificationLogger.logStep(
        step: 'payload_handle_skip',
        source: 'localNotificationService',
        detail: 'empty payload',
      );
      return;
    }

    final update = TripScheduleUpdate.tryParse(payload);
    if (update != null) {
      FcmNotificationLogger.logStep(
        step: 'payload_handle_branch',
        source: 'localNotificationService',
        payload: payload,
        detail: 'trip_schedule_update',
      );
      _pendingUpdate = update;
      _scheduleUpdateController.add(update);
      if (fromColdStart) {
        NotificationLaunchHandler.setPendingFromPayload(payload);
      } else {
        FcmNotificationLogger.logStep(
          step: 'schedule_update_tap_callback',
          source: 'localNotificationService',
        );
        _onScheduleUpdateTap?.call();
      }
      return;
    }

    if (fromColdStart) {
      FcmNotificationLogger.logStep(
        step: 'payload_handle_branch',
        source: 'localNotificationService',
        payload: payload,
        detail: 'cold_start_defer',
      );
      NotificationLaunchHandler.setPendingFromPayload(payload);
      return;
    }

    if (payload.startsWith('trip:')) {
      FcmNotificationLogger.logStep(
        step: 'payload_handle_branch',
        source: 'localNotificationService',
        payload: payload,
        detail: 'open_trip_defer_nav',
      );
      NotificationLaunchHandler.setPendingFromPayload(payload);
      NotificationLaunchHandler.navigateIfPending(isLoggedIn: true);
      return;
    }

    if (payload == 'generic') {
      FcmNotificationLogger.logStep(
        step: 'payload_handle_branch',
        source: 'localNotificationService',
        payload: payload,
        detail: 'generic_tap_callback',
      );
      _onGenericNotificationTap?.call();
    }
  }

  /// Show local notification for trip schedule update (when app is in foreground).
  /// Also adds to stream so home screen can show the card.
  Future<void> showTripScheduleUpdate(RemoteMessage message) async {
    final body = message.notification?.body ?? message.data['body'] ?? '';
    final title = message.notification?.title ??
        message.data['title'] ??
        'Trip Schedule Updated';

    final update = TripScheduleUpdate.tryParse(body);
    if (update == null) {
      FcmNotificationLogger.logStep(
        step: 'show_trip_schedule_skip',
        source: 'localNotificationService',
        message: message,
        detail: 'body not a trip schedule update',
      );
      return;
    }

    // Add to stream for in-app card
    _scheduleUpdateController.add(update);

    final bodyText = update.displayText;

    FcmNotificationLogger.logReceived(
      source: 'foreground_local_display_trip_schedule',
      message: message,
      title: title,
      body: bodyText,
    );

    // Show local notification (user sees it in foreground)
    final payload = NotificationClickAction.localNotificationPayloadFromData(
          message.data,
          source: 'showTripScheduleUpdate',
        ) ??
        body;
    FcmNotificationLogger.logStep(
      step: 'local_notification_show',
      source: 'showTripScheduleUpdate',
      message: message,
      payload: payload,
    );
    await _plugin.show(
      id: message.hashCode % 100000, // Unique ID
      title: title,
      body: bodyText,
      notificationDetails: _notificationDetailsFor(
        message,
        channelId: _tripScheduleUpdateChannelId,
        channelName: _tripScheduleUpdateChannelName,
        channelDescription: 'Notifications for trip schedule updates',
        body: bodyText,
      ),
      payload: payload,
    );
  }

  /// Show local notification for any FCM message (when app is in foreground).
  /// Used for messages that are not trip schedule updates.
  Future<void> showForegroundMessage(RemoteMessage message) async {
    final title =
        message.notification?.title ?? message.data['title'] ?? 'Notification';
    final body = message.notification?.body ?? message.data['body'] ?? '';
    FcmNotificationLogger.logReceived(
      source: 'foreground_local_display',
      message: message,
      title: title,
      body: body,
    );

    final payload = NotificationClickAction.localNotificationPayloadFromData(
          message.data,
          source: 'showForegroundMessage',
        ) ??
        'generic';
    FcmNotificationLogger.logStep(
      step: 'local_notification_show',
      source: 'showForegroundMessage',
      message: message,
      payload: payload,
    );

    await _plugin.show(
      id: message.hashCode % 100000 +
          100000, // Unique ID (offset to avoid trip IDs)
      title: title,
      body: body,
      notificationDetails: _notificationDetailsFor(
        message,
        channelId: _generalChannelId,
        channelName: _generalChannelName,
        channelDescription: 'General app notifications',
        body: body,
      ),
      payload: payload,
    );
  }

  /// Add trip schedule update to stream (e.g. when FCM notification tapped in background).
  void addTripScheduleUpdateFromMessage(RemoteMessage message) {
    final body = message.notification?.body ?? message.data['body'] ?? '';
    final update = TripScheduleUpdate.tryParse(body);
    if (update != null) {
      FcmNotificationLogger.logStep(
        step: 'schedule_update_added',
        source: 'addTripScheduleUpdateFromMessage',
        message: message,
      );
      _pendingUpdate = update;
      _scheduleUpdateController.add(update);
    } else {
      FcmNotificationLogger.logStep(
        step: 'schedule_update_skip',
        source: 'addTripScheduleUpdateFromMessage',
        message: message,
        detail: 'body not parseable',
      );
    }
  }

  void dispose() {
    _scheduleUpdateController.close();
  }
}
