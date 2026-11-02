import 'dart:async';
import 'dart:io';

import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';

import '../models/trip_schedule_update.dart';
import 'fcm_background_handler.dart';
import 'local_notification_service.dart';

/// FCM Service to handle Firebase Cloud Messaging
class FCMService {
  static final FCMService _instance = FCMService._internal();
  factory FCMService() => _instance;
  FCMService._internal();

  final FirebaseMessaging _firebaseMessaging = FirebaseMessaging.instance;
  StreamSubscription<RemoteMessage>? _messageSubscription;
  StreamSubscription<RemoteMessage>? _backgroundMessageSubscription;
  Future<void> Function(String)? _onTokenReceived;
  void Function()? _onNotificationReceived;
  String? _currentToken;

  /// Initialize FCM service. Call after LocalNotificationService.initialize().
  ///
  /// On Android 13+, runtime notification permission must be requested in context
  /// with an in-app disclosure shown immediately before the system dialog. Pass
  /// [requestRuntimeNotificationPermission]: false for Android and call
  /// [requestRuntimeNotificationPermission] after the user accepts your disclosure.
  Future<void> initialize({
    bool requestRuntimeNotificationPermission = true,
  }) async {
    try {
      if (requestRuntimeNotificationPermission) {
        await _requestRuntimeNotificationPermission();
      }

      // On iOS, APNS token may not be ready immediately. Add a short delay
      // to give the system time to register for push notifications.
      if (Platform.isIOS) {
        await Future.delayed(const Duration(milliseconds: 500));
      }

      // Get FCM token (with retry on iOS for apns-token-not-set)
      final token = await getToken();
      debugPrint('FCM Token: $token');
      _currentToken = token;

      // Send token to backend if callback is set
      if (token != null && _onTokenReceived != null) {
        try {
          await _onTokenReceived!(token);
        } catch (e) {
          debugPrint('Error sending FCM token to backend: $e');
        }
      }

      // Handle foreground messages
      _setupForegroundMessageHandler();

      // Handle token refresh
      _firebaseMessaging.onTokenRefresh.listen((newToken) {
        debugPrint('FCM Token refreshed: $newToken');
        _onTokenRefresh(newToken);
      });

      // Set up background message handler
      FirebaseMessaging.onBackgroundMessage(firebaseMessagingBackgroundHandler);

      debugPrint('FCM Service initialized successfully');
    } catch (e) {
      debugPrint('Error initializing FCM Service: $e');
    }
  }

  /// Shows the system notification permission prompt (iOS / Android 13+).
  /// Call only after presenting an in-app disclosure on Android.
  Future<NotificationSettings> requestRuntimeNotificationPermission() =>
      _requestRuntimeNotificationPermission();

  Future<NotificationSettings> _requestRuntimeNotificationPermission() async {
    final NotificationSettings settings = await _firebaseMessaging
        .requestPermission(
          alert: true,
          announcement: false,
          badge: true,
          carPlay: false,
          criticalAlert: false,
          provisional: false,
          sound: true,
        );
    debugPrint('User granted permission: ${settings.authorizationStatus}');
    return settings;
  }

  /// Get FCM token
  /// On iOS, retries when APNS token is not yet available (apns-token-not-set).
  Future<String?> getToken() async {
    const maxRetries = 5;
    const retryDelay = Duration(seconds: 1);

    for (int attempt = 0; attempt < maxRetries; attempt++) {
      try {
        _currentToken = await _firebaseMessaging.getToken();
        return _currentToken;
      } catch (e) {
        final errorStr = e.toString();
        final isApnsNotReady = errorStr.contains('apns-token-not-set');

        if (Platform.isIOS && isApnsNotReady && attempt < maxRetries - 1) {
          debugPrint(
            'APNS token not ready, retrying in ${retryDelay.inSeconds}s '
            '(attempt ${attempt + 1}/$maxRetries)',
          );
          await Future.delayed(retryDelay);
        } else {
          debugPrint('Error getting FCM token: $e');
          return null;
        }
      }
    }
    return null;
  }

  /// Get current cached FCM token (synchronous)
  String? get currentToken => _currentToken;

  /// Delete FCM token
  Future<void> deleteToken() async {
    try {
      await _firebaseMessaging.deleteToken();
      _currentToken = null;
      debugPrint('FCM Token deleted');
    } catch (e) {
      debugPrint('Error deleting FCM token: $e');
    }
  }

  /// Subscribe to a topic
  Future<void> subscribeToTopic(String topic) async {
    try {
      await _firebaseMessaging.subscribeToTopic(topic);
      debugPrint('Subscribed to topic: $topic');
    } catch (e) {
      debugPrint('Error subscribing to topic $topic: $e');
    }
  }

  /// Unsubscribe from a topic
  Future<void> unsubscribeFromTopic(String topic) async {
    try {
      await _firebaseMessaging.unsubscribeFromTopic(topic);
      debugPrint('Unsubscribed from topic: $topic');
    } catch (e) {
      debugPrint('Error unsubscribing from topic $topic: $e');
    }
  }

  /// Set up foreground message handler
  void _setupForegroundMessageHandler() {
    _messageSubscription?.cancel();
    _messageSubscription = FirebaseMessaging.onMessage.listen((
      RemoteMessage message,
    ) {
      debugPrint('Received foreground message: ${message.messageId}');
      debugPrint('Message data: ${message.data}');
      debugPrint('Message notification: ${message.notification?.title}');

      // Handle foreground message
      _onForegroundMessage(message);
    });
  }

  /// Handle foreground messages
  void _onForegroundMessage(RemoteMessage message) async {
    final title = message.notification?.title ?? message.data['title'];
    final body = message.notification?.body ?? message.data['body'] ?? '';
    debugPrint('Foreground message received: title=$title, body=$body');
    try {
      if (TripScheduleUpdate.tryParse(body) != null) {
        await LocalNotificationService.instance.showTripScheduleUpdate(message);
      } else {
        await LocalNotificationService.instance.showForegroundMessage(message);
      }
    } catch (e, st) {
      debugPrint('Error handling foreground message: $e\n$st');
    }
    // Trigger trips refresh so home page shows any new trip
    _onNotificationReceived?.call();
  }

  /// Handle token refresh
  void _onTokenRefresh(String newToken) {
    debugPrint('Token refreshed, should update backend: $newToken');
    _currentToken = newToken;
    // Send new token to backend if callback is set
    if (_onTokenReceived != null) {
      _onTokenReceived!(newToken).catchError((e) {
        debugPrint('Error sending refreshed FCM token to backend: $e');
      });
    }
  }

  /// Set callback to send FCM token to backend
  void setTokenCallback(Future<void> Function(String) callback) {
    _onTokenReceived = callback;
  }

  /// Set callback when any notification is received (foreground).
  /// Use to refresh trips so home page shows new trip cards.
  void setOnNotificationReceived(void Function() callback) {
    _onNotificationReceived = callback;
  }

  /// Handle notification tap when app is in background or terminated
  void setupNotificationTapHandler(Function(RemoteMessage) onNotificationTap) {
    _backgroundMessageSubscription?.cancel();
    _backgroundMessageSubscription = FirebaseMessaging.onMessageOpenedApp
        .listen((RemoteMessage message) {
          debugPrint('Notification tapped: ${message.messageId}');
          debugPrint('Message data: ${message.data}');
          onNotificationTap(message);
        });
  }

  /// Get initial message if app was opened from a terminated state
  Future<RemoteMessage?> getInitialMessage() async {
    try {
      return await _firebaseMessaging.getInitialMessage();
    } catch (e) {
      debugPrint('Error getting initial message: $e');
      return null;
    }
  }

  /// Manually send current FCM token to backend
  /// This is useful to call after login to ensure token is sent
  Future<void> sendTokenToBackend() async {
    final token = await getToken();
    if (token != null && _onTokenReceived != null) {
      try {
        await _onTokenReceived!(token);
        debugPrint('FCM token sent to backend manually');
      } catch (e) {
        debugPrint('Error sending FCM token to backend manually: $e');
      }
    }
  }

  /// Dispose resources
  void dispose() {
    _messageSubscription?.cancel();
    _backgroundMessageSubscription?.cancel();
  }
}
