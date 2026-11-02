import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';

/// Top-level function to handle background messages
/// This must be a top-level function, not a class method
/// It will be called when a message is received while the app is in the background
@pragma('vm:entry-point')
Future<void> firebaseMessagingBackgroundHandler(RemoteMessage message) async {
  debugPrint('Handling background message: ${message.messageId}');
  debugPrint('Message data: ${message.data}');
  debugPrint('Message notification: ${message.notification?.title}');
  debugPrint('Message notification body: ${message.notification?.body}');

  // Background messages are automatically displayed by the system
  // Additional processing can be added here if needed:
  // - Update local database/cache
  // - Sync data with backend
  // - Process notification data for app state updates
}
